using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Vectrosity;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009FC RID: 2556
	public class WaypointVisual : MonoBehaviour
	{
		// Token: 0x170010CB RID: 4299
		// (get) Token: 0x060061BA RID: 25018 RVA: 0x002DEA54 File Offset: 0x002DCC54
		private float PlayerScaleModifier
		{
			get
			{
				if (!this.isPlayerWaypoint)
				{
					return 0.5f;
				}
				return 1f;
			}
		}

		// Token: 0x170010CC RID: 4300
		// (get) Token: 0x060061BB RID: 25019 RVA: 0x002DEA69 File Offset: 0x002DCC69
		private float CoreWaypointScaleModifier
		{
			get
			{
				if (!this._isCoreWaypoint)
				{
					return 0.8f;
				}
				return 1f;
			}
		}

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x060061BC RID: 25020 RVA: 0x002DEA80 File Offset: 0x002DCC80
		// (remove) Token: 0x060061BD RID: 25021 RVA: 0x002DEAB8 File Offset: 0x002DCCB8
		public event Action OnWaypointMouseOverBegin;

		// Token: 0x1400002D RID: 45
		// (add) Token: 0x060061BE RID: 25022 RVA: 0x002DEAF0 File Offset: 0x002DCCF0
		// (remove) Token: 0x060061BF RID: 25023 RVA: 0x002DEB28 File Offset: 0x002DCD28
		public event Action OnWaypointMouseOverEnd;

		// Token: 0x170010CD RID: 4301
		// (get) Token: 0x060061C0 RID: 25024 RVA: 0x002DEB5D File Offset: 0x002DCD5D
		public bool IsOverlapping
		{
			get
			{
				return this._isOverlapping;
			}
		}

		// Token: 0x170010CE RID: 4302
		// (get) Token: 0x060061C1 RID: 25025 RVA: 0x002DEB65 File Offset: 0x002DCD65
		public bool IsVisible
		{
			get
			{
				return this._renderer.isVisible;
			}
		}

		// Token: 0x170010CF RID: 4303
		// (get) Token: 0x060061C2 RID: 25026 RVA: 0x002DEB72 File Offset: 0x002DCD72
		public GameObject GizmoRoot
		{
			get
			{
				return this.gizmoRoot;
			}
		}

		// Token: 0x170010D0 RID: 4304
		// (set) Token: 0x060061C3 RID: 25027 RVA: 0x002DEB7A File Offset: 0x002DCD7A
		public float ColorInterpolationRatio
		{
			set
			{
				this._colorInterpolationRatio = Mathf.Clamp01(value);
				this.SelectBaseColor();
				this.ShowBaseColor();
			}
		}

		// Token: 0x170010D1 RID: 4305
		// (get) Token: 0x060061C4 RID: 25028 RVA: 0x002DEB94 File Offset: 0x002DCD94
		// (set) Token: 0x060061C5 RID: 25029 RVA: 0x002DEB9C File Offset: 0x002DCD9C
		public int BaseColorIndex
		{
			get
			{
				return this._baseColorIndex;
			}
			private set
			{
				this._baseColorIndex = Mathf.Clamp(value, 1, this.colors.Count - 1);
			}
		}

		// Token: 0x170010D2 RID: 4306
		// (get) Token: 0x060061C6 RID: 25030 RVA: 0x002DEBB8 File Offset: 0x002DCDB8
		public Color BaseColor
		{
			get
			{
				return this._baseColor;
			}
		}

		// Token: 0x170010D3 RID: 4307
		// (get) Token: 0x060061C7 RID: 25031 RVA: 0x002DEBC0 File Offset: 0x002DCDC0
		public float BaseColorAlpha
		{
			get
			{
				return this._baseColor.a;
			}
		}

		// Token: 0x170010D4 RID: 4308
		// (get) Token: 0x060061C8 RID: 25032 RVA: 0x002DEBD0 File Offset: 0x002DCDD0
		public float ScaledRadius
		{
			get
			{
				return base.transform.localScale.magnitude;
			}
		}

		// Token: 0x060061C9 RID: 25033 RVA: 0x002DEBF0 File Offset: 0x002DCDF0
		public static WaypointVisual Create(Transform waypointVisualPrefab, int colorIndex, Transform parent, TISpaceShipState shipState)
		{
			WaypointVisual component = global::UnityEngine.Object.Instantiate<Transform>(waypointVisualPrefab, Vector3.zero, Quaternion.identity).GetComponent<WaypointVisual>();
			component.Initialize(colorIndex, true, shipState);
			component.transform.SetParent(parent, false);
			component.transform.SetLayer(16, true);
			component.isPlayerWaypoint = true;
			component._isPlacementWaypoint = true;
			component.name = "WaypointPlacementVisual";
			return component;
		}

		// Token: 0x060061CA RID: 25034 RVA: 0x002DEC50 File Offset: 0x002DCE50
		public static WaypointVisual Create(IMovableWaypoint waypoint, Transform waypointVisualPrefab, int colorIndex, Transform parent, Vector3 initialForward, bool isCoreWaypoint, TISpaceShipState shipState)
		{
			WaypointVisual component = global::UnityEngine.Object.Instantiate<Transform>(waypointVisualPrefab, waypoint.Position, Quaternion.identity).GetComponent<WaypointVisual>();
			component.Initialize(colorIndex, isCoreWaypoint, shipState);
			component.transform.SetParent(parent, false);
			component.transform.SetLayer(16, true);
			component.transform.forward = initialForward;
			component.name = "WaypointPlacementVisual";
			return component;
		}

		// Token: 0x060061CB RID: 25035 RVA: 0x002DECB4 File Offset: 0x002DCEB4
		private void Initialize(int baseColorIndex, bool isCoreWaypoint, TISpaceShipState shipState)
		{
			this._spaceCombatUiLayerMask = LayerMask.GetMask(new string[] { "Space Combat UI" });
			base.name = "WaypointPlacementVisual";
			this._heightLineRenderer = new WaypointVisual.HeightLineRenderer(base.name);
			this._renderer = base.GetComponent<Renderer>();
			base.StartCoroutine(this.SetHeightlineParent());
			this._camera = GameControl.spaceCombat.mainCamera;
			this._cameraTransform = this._camera.transform;
			this._spaceCombatCameraController = this._camera.GetComponent<SpaceCombatCameraController>();
			this.BaseColorIndex = baseColorIndex;
			this._isCoreWaypoint = isCoreWaypoint;
			this._shipState = shipState;
			this.adjustedMaxScale = this.maxScale * GameControl.spaceCombat.modelScalingFactor;
			this.SelectBaseColor();
			this.InitializeWaypointScale();
			this.InitializeWaypointColors();
			if (this.waypointUI != null)
			{
				this.waypointUI.Initialize(this, shipState);
			}
			if (baseColorIndex == 0)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x060061CC RID: 25036 RVA: 0x002DEDA9 File Offset: 0x002DCFA9
		public Vector3 GetShipPosition()
		{
			if (this._shipTransform == null)
			{
				this._shipTransform = GameControl.spaceCombat.combatantLookup[this._shipState].ref_shipController.transform;
			}
			return this._shipTransform.position;
		}

		// Token: 0x060061CD RID: 25037 RVA: 0x002DEDE9 File Offset: 0x002DCFE9
		private IEnumerator SetHeightlineParent()
		{
			yield return null;
			this._heightLineRenderer.thisT.SetParent(base.transform.parent);
			yield break;
		}

		// Token: 0x060061CE RID: 25038 RVA: 0x002DEDF8 File Offset: 0x002DCFF8
		public void DecrementColorIndex()
		{
			this.BaseColorIndex = ((this._baseColorIndex == 1) ? (this.colors.Count - 1) : (this._baseColorIndex - 1));
			this.SelectBaseColor();
		}

		// Token: 0x060061CF RID: 25039 RVA: 0x002DEE26 File Offset: 0x002DD026
		public void SetColorIndex(int index, float colorInterpolationRatio)
		{
			this.BaseColorIndex = index;
			this.ColorInterpolationRatio = colorInterpolationRatio;
		}

		// Token: 0x060061D0 RID: 25040 RVA: 0x002DEE38 File Offset: 0x002DD038
		private void SelectBaseColor()
		{
			if ((this._isCoreWaypoint && !this._isPlacementWaypoint) || this._baseColorIndex == 1)
			{
				this._baseColor = this.colors[this._baseColorIndex];
				return;
			}
			this._baseColor = Color.Lerp(this.colors[this._baseColorIndex - 1], this.colors[this._baseColorIndex], this._colorInterpolationRatio);
		}

		// Token: 0x060061D1 RID: 25041 RVA: 0x002DEEAC File Offset: 0x002DD0AC
		private void InitializeWaypointScale()
		{
			float num = this.CoreWaypointScaleModifier * this.PlayerScaleModifier;
			float num2 = 0.00875f * num;
			this._initialScale = base.transform.localScale * num2;
			this.SetInputHandling(false);
			this.UpdateWaypointScale();
		}

		// Token: 0x060061D2 RID: 25042 RVA: 0x002DEEF4 File Offset: 0x002DD0F4
		public void UpdateWaypointScale()
		{
			Vector3 vector = this._initialScale * new Plane(this._cameraTransform.forward, this._cameraTransform.position).GetDistanceToPoint(base.transform.position);
			vector.x = Mathf.Clamp(vector.x, this.minScale, this.adjustedMaxScale);
			vector.y = Mathf.Clamp(vector.y, this.minScale, this.adjustedMaxScale);
			vector.z = Mathf.Clamp(vector.z, this.minScale, this.adjustedMaxScale);
			base.transform.localScale = vector * this._inputHandlingScaleFactor;
		}

		// Token: 0x060061D3 RID: 25043 RVA: 0x002DEFAC File Offset: 0x002DD1AC
		private void Update()
		{
			this.UpdateWaypointScale();
		}

		// Token: 0x060061D4 RID: 25044 RVA: 0x002DEFB4 File Offset: 0x002DD1B4
		private void LateUpdate()
		{
			if (this._isLockRotationSet)
			{
				this.BurnGizmo.transform.rotation = this._lockRotation;
			}
			else if (this.isPlayerWaypoint)
			{
				this.MovementGizmo.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
				this.LateralGizmo.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
				this.AltitudeGizmo.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
			}
			this._isOverlapping = false;
			if (this.isPlayerWaypoint && this.BaseColorIndex != 1)
			{
				foreach (Collider collider in Physics.OverlapSphere(base.transform.position, base.transform.localScale.x * this.capsuleCollider.radius * (1f - this._overlappingPercentage), this.waypointOcclusionLayermask, QueryTriggerInteraction.Collide))
				{
					ProjectileController component = collider.GetComponent<ProjectileController>();
					if (collider != this.capsuleCollider && component == null)
					{
						this._isOverlapping = true;
						return;
					}
				}
			}
		}

		// Token: 0x060061D5 RID: 25045 RVA: 0x002DF0E0 File Offset: 0x002DD2E0
		private void InitializeWaypointColors()
		{
			this._hoverEmissionColor = this.waypointHoverColor * Mathf.LinearToGammaSpace(1f);
			this._lockedEmissionColor = this.waypointLockedColor * Mathf.LinearToGammaSpace(1f);
			if (Error.IsNull<Renderer>(this._renderer, "No Renderer Found", Array.Empty<object>()))
			{
				return;
			}
			this._emissionColorId = Shader.PropertyToID("_EmissionColor");
			this.ShowBaseColor();
		}

		// Token: 0x060061D6 RID: 25046 RVA: 0x002DF154 File Offset: 0x002DD354
		public void ShowBaseColor()
		{
			Material[] materials = this._renderer.materials;
			if (this._renderer.materials.Length == 2)
			{
				materials[1].color = this._baseColor;
				materials[0].color = this._baseColor;
				materials[0].SetColor("_Color", Color.white * this._baseColor.a * Mathf.LinearToGammaSpace(1f));
				materials[0].SetColor(this._emissionColorId, this._baseColor * Mathf.LinearToGammaSpace(1f));
			}
			else
			{
				materials[0].SetColor("_Color", Color.white * this._baseColor.a * Mathf.LinearToGammaSpace(1f));
				materials[0].SetColor(this._emissionColorId, this._baseColor * Mathf.LinearToGammaSpace(1f));
			}
			if (this.rotationAxisRenderer.Length != 0)
			{
				for (int i = 0; i < this.rotationAxisRenderer.Length; i++)
				{
					this.rotationAxisRenderer[i].material.color = this._baseColor;
				}
			}
		}

		// Token: 0x060061D7 RID: 25047 RVA: 0x002DF278 File Offset: 0x002DD478
		public void ShowHighlightColor()
		{
			Material[] materials = this._renderer.materials;
			if (this._renderer.materials.Length == 2)
			{
				materials[1].color = this.waypointHoverColor;
				materials[0].color = this.waypointHoverColor;
				materials[0].SetColor(this._emissionColorId, this._hoverEmissionColor);
				return;
			}
			materials[0].SetColor(this._emissionColorId, this._hoverEmissionColor);
		}

		// Token: 0x060061D8 RID: 25048 RVA: 0x002DF2E8 File Offset: 0x002DD4E8
		public void ShowLockedColor()
		{
			Material[] materials = this._renderer.materials;
			if (materials.Length == 2)
			{
				materials[1].color = this.waypointLockedColor;
				materials[0].color = this.waypointLockedColor;
				materials[0].SetColor(this._emissionColorId, this.waypointLockedColor);
			}
			else
			{
				materials[0].SetColor(this._emissionColorId, this.waypointLockedColor);
			}
			if (this.rotationAxisRenderer.Length != 0)
			{
				for (int i = 0; i < this.rotationAxisRenderer.Length; i++)
				{
					this.rotationAxisRenderer[i].material.color = this.waypointLockedColor;
				}
			}
		}

		// Token: 0x060061D9 RID: 25049 RVA: 0x002DF384 File Offset: 0x002DD584
		public void ShowSystemFailureLockedColor()
		{
			Material[] materials = this._renderer.materials;
			if (materials.Length == 2)
			{
				materials[1].color = this.waypointSystemLockedColor;
				materials[0].color = this.waypointSystemLockedColor;
				materials[0].SetColor(this._emissionColorId, this.waypointSystemLockedColor);
				return;
			}
			materials[0].SetColor(this._emissionColorId, this.waypointSystemLockedColor);
		}

		// Token: 0x060061DA RID: 25050 RVA: 0x002DF3E8 File Offset: 0x002DD5E8
		public void SetInputHandling(bool isActive)
		{
			this._inputHandlingScaleFactor = this.CoreWaypointScaleModifier * this.InputScaleModifier(isActive);
		}

		// Token: 0x060061DB RID: 25051 RVA: 0x002DF3FE File Offset: 0x002DD5FE
		private float InputScaleModifier(bool isActive)
		{
			if (!isActive)
			{
				return 1f;
			}
			return 1.5f;
		}

		// Token: 0x060061DC RID: 25052 RVA: 0x002DF40E File Offset: 0x002DD60E
		public void SetRendererEnabled(bool setActive)
		{
			this._renderer.enabled = setActive;
			if (this.waypointUI != null)
			{
				this.waypointUI.ToggleVisibility(this._renderer.enabled);
			}
		}

		// Token: 0x060061DD RID: 25053 RVA: 0x002DF440 File Offset: 0x002DD640
		public void ToggleRenderer()
		{
			this._renderer.enabled = !this._renderer.enabled;
			if (this.waypointUI != null)
			{
				this.waypointUI.ToggleVisibility(this._renderer.enabled);
			}
		}

		// Token: 0x060061DE RID: 25054 RVA: 0x002DF47F File Offset: 0x002DD67F
		public void ToggleHighlight()
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this._isHighlighted = !this._isHighlighted;
			if (this._isHighlighted)
			{
				this.ShowHighlightColor();
				return;
			}
			this.ShowBaseColor();
		}

		// Token: 0x060061DF RID: 25055 RVA: 0x002DF4AE File Offset: 0x002DD6AE
		private void OnMouseEnter()
		{
			this.HandleMouseEnter();
		}

		// Token: 0x060061E0 RID: 25056 RVA: 0x002DF4B6 File Offset: 0x002DD6B6
		private void OnMouseExit()
		{
			this.HandleMouseExit();
		}

		// Token: 0x060061E1 RID: 25057 RVA: 0x002DF4C0 File Offset: 0x002DD6C0
		public void HandleMouseEnter()
		{
			if (GameControl.spaceCombat.IsHandlingWaypointInput || GameControl.spaceCombat.IsDragSelecting || GameControl.spaceCombat.combatEnded || !TIInputManager.acceptingInput || TIInputManager.IsCameraMovementKeyPressed || !this._renderer.enabled)
			{
				return;
			}
			if (this._spaceCombatCameraController.IsDragging)
			{
				this._isInputHandlingDelayedForCameraDrag = true;
				return;
			}
			Action onWaypointMouseOverBegin = this.OnWaypointMouseOverBegin;
			if (onWaypointMouseOverBegin == null)
			{
				return;
			}
			onWaypointMouseOverBegin();
		}

		// Token: 0x060061E2 RID: 25058 RVA: 0x002DF532 File Offset: 0x002DD732
		public void HandleMouseExit()
		{
			if (!this._isInputHandlingDelayedForCameraDrag)
			{
				Action onWaypointMouseOverEnd = this.OnWaypointMouseOverEnd;
				if (onWaypointMouseOverEnd != null)
				{
					onWaypointMouseOverEnd();
				}
			}
			this._isInputHandlingDelayedForCameraDrag = false;
		}

		// Token: 0x060061E3 RID: 25059 RVA: 0x002DF554 File Offset: 0x002DD754
		public void SetPositionRotation(Vector3 position, Quaternion rotation)
		{
			base.transform.localPosition = position;
			base.transform.rotation = rotation;
			this._heightLineRenderer.SetRenderPosition(position);
		}

		// Token: 0x060061E4 RID: 25060 RVA: 0x002DF57A File Offset: 0x002DD77A
		public void ToggleHeightLine(bool shouldRenderLine)
		{
			this._heightLineRenderer.ToggleRenderState(shouldRenderLine);
			if (this.isPlayerWaypoint)
			{
				this.ToggleWaypointDVCost(shouldRenderLine);
			}
		}

		// Token: 0x060061E5 RID: 25061 RVA: 0x002DF597 File Offset: 0x002DD797
		public void ToggleWaypointDVCost(bool shouldShow)
		{
			this.waypointUI.ToggleDVText(shouldShow);
		}

		// Token: 0x060061E6 RID: 25062 RVA: 0x002DF5A5 File Offset: 0x002DD7A5
		public void ToggleWaypointCollisionWarning(bool shouldShow)
		{
			this.waypointUI.ToggleCollisionWarning(shouldShow);
		}

		// Token: 0x060061E7 RID: 25063 RVA: 0x002DF5B3 File Offset: 0x002DD7B3
		public void ShowYawGizmo(bool value)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.YawRotationGizmo.gameObject.SetActive(value);
		}

		// Token: 0x060061E8 RID: 25064 RVA: 0x002DF5CF File Offset: 0x002DD7CF
		public void ShowYawGizmoHighlight(bool value)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.YawRotationGizmo.SetGizmoHighlight(value);
		}

		// Token: 0x060061E9 RID: 25065 RVA: 0x002DF5E6 File Offset: 0x002DD7E6
		public void ShowPitchGizmo(bool value)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.PitchRotationGizmo.gameObject.SetActive(value);
		}

		// Token: 0x060061EA RID: 25066 RVA: 0x002DF602 File Offset: 0x002DD802
		public void ShowPitchGizmoHighlight(bool value)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.PitchRotationGizmo.SetGizmoHighlight(value);
		}

		// Token: 0x060061EB RID: 25067 RVA: 0x002DF619 File Offset: 0x002DD819
		public void ShowRollGizmo(bool value)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.RollRotationGizmo.gameObject.SetActive(value);
		}

		// Token: 0x060061EC RID: 25068 RVA: 0x002DF635 File Offset: 0x002DD835
		public void ShowRollGizmoHighlight(bool value)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.RollRotationGizmo.SetGizmoHighlight(value);
		}

		// Token: 0x060061ED RID: 25069 RVA: 0x002DF64C File Offset: 0x002DD84C
		public void ShowMovementGizmo(bool value, bool highlight = false)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.MovementGizmo.gameObject.SetActive(value);
			this.MovementGizmo.SetGizmoHighlight(highlight);
		}

		// Token: 0x060061EE RID: 25070 RVA: 0x002DF674 File Offset: 0x002DD874
		public void ShowMovementInvalid(bool value)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.MovementGizmo.SetGizmoInvalid(value);
		}

		// Token: 0x060061EF RID: 25071 RVA: 0x002DF68B File Offset: 0x002DD88B
		public void ShowAltitudeGizmo(bool value)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.AltitudeGizmo.gameObject.SetActive(value);
		}

		// Token: 0x060061F0 RID: 25072 RVA: 0x002DF6A7 File Offset: 0x002DD8A7
		public void ShowAltitudeGizmoHighlight(bool highlight)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.AltitudeGizmo.SetGizmoHighlight(highlight);
		}

		// Token: 0x060061F1 RID: 25073 RVA: 0x002DF6BE File Offset: 0x002DD8BE
		public void ShowAltitudeInvalid(bool value)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.AltitudeGizmo.SetGizmoInvalid(value);
		}

		// Token: 0x060061F2 RID: 25074 RVA: 0x002DF6D5 File Offset: 0x002DD8D5
		public void ShowLateralGizmo(bool value)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.LateralGizmo.gameObject.SetActive(value);
		}

		// Token: 0x060061F3 RID: 25075 RVA: 0x002DF6F1 File Offset: 0x002DD8F1
		public void ShowLateralGizmoHighlight(bool highlight)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.LateralGizmo.SetGizmoHighlight(highlight);
		}

		// Token: 0x060061F4 RID: 25076 RVA: 0x002DF708 File Offset: 0x002DD908
		public void ShowLateralInvalid(bool value)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.LateralGizmo.SetGizmoInvalid(value);
		}

		// Token: 0x060061F5 RID: 25077 RVA: 0x002DF71F File Offset: 0x002DD91F
		public void ShowBurnGizmo(bool value)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.LockMovementGizmoRotation(value);
			this.BurnGizmo.gameObject.SetActive(value);
		}

		// Token: 0x060061F6 RID: 25078 RVA: 0x002DF742 File Offset: 0x002DD942
		public void ShowBurnHighlight(bool highlight)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.LockMovementGizmoRotation(highlight);
			this.BurnGizmo.SetGizmoHighlight(highlight);
		}

		// Token: 0x060061F7 RID: 25079 RVA: 0x002DF760 File Offset: 0x002DD960
		public void ShowBurnInvalid(bool value)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			this.LockMovementGizmoRotation(value);
			this.BurnGizmo.SetGizmoInvalid(value);
		}

		// Token: 0x060061F8 RID: 25080 RVA: 0x002DF780 File Offset: 0x002DD980
		public void LockMovementGizmoRotation(bool value)
		{
			if (!this.isPlayerWaypoint)
			{
				return;
			}
			if (!this._isLockRotationSet && value)
			{
				this._lockRotation = base.transform.rotation;
				this._isLockRotationSet = true;
			}
			if (!value)
			{
				this._isLockRotationSet = false;
				this.MovementGizmo.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
				this.LateralGizmo.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
				this.AltitudeGizmo.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
			}
		}

		// Token: 0x060061F9 RID: 25081 RVA: 0x002DF841 File Offset: 0x002DDA41
		public void OnDestroy()
		{
			this._heightLineRenderer.Destroy();
			this._heightLineRenderer = null;
		}

		// Token: 0x040044A1 RID: 17569
		[SerializeField]
		private List<Color> colors = new List<Color>();

		// Token: 0x040044A2 RID: 17570
		[SerializeField]
		private Color waypointHoverColor = Color.white;

		// Token: 0x040044A3 RID: 17571
		[SerializeField]
		private Color waypointLockedColor = Color.white;

		// Token: 0x040044A4 RID: 17572
		[SerializeField]
		private Color waypointSystemLockedColor = Color.white;

		// Token: 0x040044A5 RID: 17573
		[SerializeField]
		private float minScale;

		// Token: 0x040044A6 RID: 17574
		[SerializeField]
		private float maxScale;

		// Token: 0x040044A7 RID: 17575
		private float adjustedMaxScale;

		// Token: 0x040044A8 RID: 17576
		public bool isPlayerWaypoint;

		// Token: 0x040044A9 RID: 17577
		[SerializeField]
		private GameObject gizmoRoot;

		// Token: 0x040044AA RID: 17578
		[SerializeField]
		private Renderer[] rotationAxisRenderer;

		// Token: 0x040044AB RID: 17579
		[SerializeField]
		private CapsuleCollider capsuleCollider;

		// Token: 0x040044AC RID: 17580
		[SerializeField]
		private LayerMask waypointOcclusionLayermask;

		// Token: 0x040044AD RID: 17581
		public WaypointUIController waypointUI;

		// Token: 0x040044AE RID: 17582
		private const string ALBEDO_COLOR = "_Color";

		// Token: 0x040044AF RID: 17583
		private const string EMISSION_COLOR = "_EmissionColor";

		// Token: 0x040044B0 RID: 17584
		private const string NAME = "WaypointPlacementVisual";

		// Token: 0x040044B1 RID: 17585
		private const float BASE_INITIAL_SCALING_FACTOR = 0.00875f;

		// Token: 0x040044B2 RID: 17586
		private const float ACTIVE_INPUT_SCALE_MODIFIER = 1.5f;

		// Token: 0x040044B3 RID: 17587
		private const float NO_INPUT_SCALE_MODIFIER = 1f;

		// Token: 0x040044B4 RID: 17588
		private const float PLAYER_SCALE_MODIFIER = 1f;

		// Token: 0x040044B5 RID: 17589
		private const float NON_PLAYER_SCALE_MODIFIER = 0.5f;

		// Token: 0x040044B6 RID: 17590
		private const float CORE_WAYPOINT_SCALE_MODIFER = 1f;

		// Token: 0x040044B7 RID: 17591
		private const float NON_CORE_WAYPOINT_SCALE_MODIFIER = 0.8f;

		// Token: 0x040044BA RID: 17594
		private Renderer _renderer;

		// Token: 0x040044BB RID: 17595
		private Camera _camera;

		// Token: 0x040044BC RID: 17596
		private Transform _cameraTransform;

		// Token: 0x040044BD RID: 17597
		private SpaceCombatCameraController _spaceCombatCameraController;

		// Token: 0x040044BE RID: 17598
		private WaypointVisual.HeightLineRenderer _heightLineRenderer;

		// Token: 0x040044BF RID: 17599
		private Vector3 _initialScale;

		// Token: 0x040044C0 RID: 17600
		private int _emissionColorId;

		// Token: 0x040044C1 RID: 17601
		private Color _hoverEmissionColor;

		// Token: 0x040044C2 RID: 17602
		private Color _lockedEmissionColor;

		// Token: 0x040044C3 RID: 17603
		private bool _isHighlighted;

		// Token: 0x040044C4 RID: 17604
		private bool _isInputHandlingDelayedForCameraDrag;

		// Token: 0x040044C5 RID: 17605
		private float _scalingFactor;

		// Token: 0x040044C6 RID: 17606
		private float _inputHandlingScaleFactor;

		// Token: 0x040044C7 RID: 17607
		private bool _isCoreWaypoint;

		// Token: 0x040044C8 RID: 17608
		public bool _isPlacementWaypoint;

		// Token: 0x040044C9 RID: 17609
		private bool _containsPartialWaypoint;

		// Token: 0x040044CA RID: 17610
		private TISpaceShipState _shipState;

		// Token: 0x040044CB RID: 17611
		private Transform _shipTransform;

		// Token: 0x040044CC RID: 17612
		private float _overlappingPercentage = 0.6f;

		// Token: 0x040044CD RID: 17613
		private bool _isOverlapping;

		// Token: 0x040044CE RID: 17614
		[Header("Gizmo Visuals")]
		public WaypointGizmoVisual PitchRotationGizmo;

		// Token: 0x040044CF RID: 17615
		public WaypointGizmoVisual YawRotationGizmo;

		// Token: 0x040044D0 RID: 17616
		public WaypointGizmoVisual RollRotationGizmo;

		// Token: 0x040044D1 RID: 17617
		public WaypointGizmoVisual MovementGizmo;

		// Token: 0x040044D2 RID: 17618
		public WaypointGizmoVisual AltitudeGizmo;

		// Token: 0x040044D3 RID: 17619
		public WaypointGizmoVisual LateralGizmo;

		// Token: 0x040044D4 RID: 17620
		public WaypointGizmoVisual BurnGizmo;

		// Token: 0x040044D5 RID: 17621
		private Quaternion _lockRotation;

		// Token: 0x040044D6 RID: 17622
		private bool _isLockRotationSet;

		// Token: 0x040044D7 RID: 17623
		private float _colorInterpolationRatio = 0.5f;

		// Token: 0x040044D8 RID: 17624
		private int _baseColorIndex;

		// Token: 0x040044D9 RID: 17625
		private Color _baseColor;

		// Token: 0x040044DA RID: 17626
		private int _spaceCombatUiLayerMask;

		// Token: 0x0200138C RID: 5004
		private class HeightLineRenderer
		{
			// Token: 0x06009189 RID: 37257 RVA: 0x00347918 File Offset: 0x00345B18
			public HeightLineRenderer(string name)
			{
				WaypointVisual.HeightLineRenderer.s_lineCount++;
				string text = string.Format("{0}_{1}_{2}", name, "HeightLine", WaypointVisual.HeightLineRenderer.s_lineCount);
				this._line = new VectorLine(text, new List<Vector3>(), 1f, LineType.Continuous)
				{
					layer = LayerMask.NameToLayer("Space Combat UI"),
					color = this._defaultColor
				};
				this._line.SetWidth(0.3f);
				this._line.Draw3DAuto();
				this._line.active = false;
				this.thisT = this._line.rectTransform.transform;
				MeshRenderer component = this._line.rectTransform.gameObject.GetComponent<MeshRenderer>();
				component.receiveShadows = false;
				component.shadowCastingMode = ShadowCastingMode.Off;
				GameControl.spaceCombat.container.Add(text, this._line.rectTransform.gameObject, false, false);
			}

			// Token: 0x0600918A RID: 37258 RVA: 0x00347A28 File Offset: 0x00345C28
			public void SetRenderPosition(Vector3 waypointPosition)
			{
				this._line.points3.Clear();
				if (waypointPosition.y == 0f)
				{
					return;
				}
				this._line.points3.Add(waypointPosition);
				this._line.points3.Add(new Vector3(waypointPosition.x, 0f, waypointPosition.z));
				this._line.Draw3DAuto();
			}

			// Token: 0x0600918B RID: 37259 RVA: 0x00347A95 File Offset: 0x00345C95
			public void ToggleRenderState(bool shouldRender)
			{
				this._line.active = shouldRender;
			}

			// Token: 0x0600918C RID: 37260 RVA: 0x00347AA3 File Offset: 0x00345CA3
			public void Destroy()
			{
				VectorLine.Destroy(ref this._line);
			}

			// Token: 0x040071D2 RID: 29138
			private const string SPACE_COMBAT_UI = "Space Combat UI";

			// Token: 0x040071D3 RID: 29139
			private const string HEIGHT_LINE_RENDERER = "HeightLine";

			// Token: 0x040071D4 RID: 29140
			private static int s_lineCount;

			// Token: 0x040071D5 RID: 29141
			private readonly Color _defaultColor = new Color(91f, 109f, 113f, 255f);

			// Token: 0x040071D6 RID: 29142
			private VectorLine _line;

			// Token: 0x040071D7 RID: 29143
			public Transform thisT;
		}
	}
}
