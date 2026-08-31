using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Settings;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Systems.Camera
{
	// Token: 0x020009B0 RID: 2480
	[UpdateInGroup(typeof(PipelineStages.RenderStage))]
	[UpdateAfter(typeof(SpaceObjectPositioning))]
	public class CameraManager : ComponentSystem
	{
		// Token: 0x17001004 RID: 4100
		// (get) Token: 0x06005D79 RID: 23929 RVA: 0x002C885E File Offset: 0x002C6A5E
		public GameObject GameObject
		{
			get
			{
				return this.unityCamera.gameObject;
			}
		}

		// Token: 0x17001005 RID: 4101
		// (get) Token: 0x06005D7A RID: 23930 RVA: 0x002C886B File Offset: 0x002C6A6B
		public Transform Transform
		{
			get
			{
				return this.GameObject.transform;
			}
		}

		// Token: 0x06005D7B RID: 23931 RVA: 0x002C8878 File Offset: 0x002C6A78
		private void CacheFocalPoint()
		{
			this.cachedFocalPoint = this.selection.spaceObjectStateSelected.controller.SpaceObject.Position;
			this.focalPointCachedFrame = TIFrameCounter.FrameCount;
			this.focalPointCachedSpaceObjectController = this.selection.SpaceObjectController;
		}

		// Token: 0x17001006 RID: 4102
		// (get) Token: 0x06005D7C RID: 23932 RVA: 0x002C88B8 File Offset: 0x002C6AB8
		public Vector3d FocalPoint
		{
			get
			{
				if (GameControl.loadcycle100)
				{
					TISpaceObjectState spaceObjectStateSelected = this.selection.spaceObjectStateSelected;
					if (!(((spaceObjectStateSelected != null) ? spaceObjectStateSelected.controller : null) == null))
					{
						if (this.focalPointCachedFrame != TIFrameCounter.FrameCount || this.focalPointCachedSpaceObjectController != this.selection.SpaceObjectController)
						{
							this.CacheFocalPoint();
						}
						return this.cachedFocalPoint;
					}
				}
				return Vector3d.zero;
			}
		}

		// Token: 0x17001007 RID: 4103
		// (get) Token: 0x06005D7D RID: 23933 RVA: 0x002C8922 File Offset: 0x002C6B22
		// (set) Token: 0x06005D7E RID: 23934 RVA: 0x002C894C File Offset: 0x002C6B4C
		public SVector3d TargetSpherical
		{
			get
			{
				if (this.UseSurfaceRotation)
				{
					return this.targetSpherical;
				}
				return CameraManager.ComputeSphericalCoordinates(this.TargetPosition, this.FocalPoint, this.SurfaceRotation);
			}
			set
			{
				if (this.UseSurfaceRotation)
				{
					this.targetSpherical = value;
					return;
				}
				Vector3d vector3d = CameraManager.ComputePosition(value, this.FocalPoint, this.SurfaceRotation);
				this.targetSpherical = CameraManager.ComputeSphericalCoordinates(vector3d, this.FocalPoint, this.SurfaceRotation);
			}
		}

		// Token: 0x17001008 RID: 4104
		// (get) Token: 0x06005D7F RID: 23935 RVA: 0x002C8994 File Offset: 0x002C6B94
		// (set) Token: 0x06005D80 RID: 23936 RVA: 0x002C899C File Offset: 0x002C6B9C
		public SVector3d Spherical
		{
			get
			{
				return this.spherical;
			}
			set
			{
				this.spherical = value;
				this.CachePosition();
			}
		}

		// Token: 0x17001009 RID: 4105
		// (get) Token: 0x06005D81 RID: 23937 RVA: 0x002C89AB File Offset: 0x002C6BAB
		public Vector3d TargetPosition
		{
			get
			{
				return CameraManager.ComputePosition(this.targetSpherical, this.FocalPoint, this.SurfaceRotation);
			}
		}

		// Token: 0x1700100A RID: 4106
		// (get) Token: 0x06005D82 RID: 23938 RVA: 0x002C89C4 File Offset: 0x002C6BC4
		public Vector3d Position
		{
			get
			{
				return this.cachedPosition;
			}
		}

		// Token: 0x1700100B RID: 4107
		// (get) Token: 0x06005D83 RID: 23939 RVA: 0x002C89CC File Offset: 0x002C6BCC
		public SurfacePosition SurfacePosition
		{
			get
			{
				return new SurfacePosition
				{
					Lat = 90.0 - this.Spherical.polar * 57.29577951308232,
					Lng = this.Spherical.azimuth
				};
			}
		}

		// Token: 0x1700100C RID: 4108
		// (get) Token: 0x06005D84 RID: 23940 RVA: 0x002C8A1C File Offset: 0x002C6C1C
		public Vector3 WorldForward
		{
			get
			{
				return (Vector3)(this.FocalPoint - this.Position).normalized;
			}
		}

		// Token: 0x1700100D RID: 4109
		// (get) Token: 0x06005D85 RID: 23941 RVA: 0x002C8A47 File Offset: 0x002C6C47
		// (set) Token: 0x06005D86 RID: 23942 RVA: 0x002C8A4F File Offset: 0x002C6C4F
		public Vector3 WorldUp { get; private set; }

		// Token: 0x1700100E RID: 4110
		// (get) Token: 0x06005D87 RID: 23943 RVA: 0x002C8A58 File Offset: 0x002C6C58
		public Vector3 Forward
		{
			get
			{
				return Quaternion.Inverse(this.SurfaceRotation) * this.WorldForward.XZY();
			}
		}

		// Token: 0x1700100F RID: 4111
		// (get) Token: 0x06005D88 RID: 23944 RVA: 0x002C8A75 File Offset: 0x002C6C75
		private bool IsTransitioningUp
		{
			get
			{
				return this.UpTransitionTimeElapsed < this.UpTransitionDuration;
			}
		}

		// Token: 0x17001010 RID: 4112
		// (get) Token: 0x06005D89 RID: 23945 RVA: 0x002C8A85 File Offset: 0x002C6C85
		public Vector3 Up
		{
			get
			{
				return Quaternion.Inverse(this.SurfaceRotation) * this.WorldUp.XZY();
			}
		}

		// Token: 0x17001011 RID: 4113
		// (get) Token: 0x06005D8A RID: 23946 RVA: 0x002C8AA2 File Offset: 0x002C6CA2
		public Quaternion TargetLookRotation
		{
			get
			{
				return (Quaternion)Quaterniond.LookRotation(this.Forward, this.Up);
			}
		}

		// Token: 0x17001012 RID: 4114
		// (get) Token: 0x06005D8B RID: 23947 RVA: 0x002C8AC4 File Offset: 0x002C6CC4
		public Quaternion BillboardRotation
		{
			get
			{
				return this.Transform.rotation;
			}
		}

		// Token: 0x17001013 RID: 4115
		// (get) Token: 0x06005D8C RID: 23948 RVA: 0x002C8AD4 File Offset: 0x002C6CD4
		public bool UseSurfaceRotation
		{
			get
			{
				return (!(this.selection.spaceObjectStateSelected != null) || !(this.selection.spaceObjectStateSelected.ref_spaceBody != null) || this.selection.spaceObjectStateSelected.ref_spaceBody.isEarth || this.selection.spaceObjectStateSelected.ref_spaceBody.habSites.Any<TIHabSiteState>()) && this.LOD == CameraManagerLOD.Surface;
			}
		}

		// Token: 0x17001014 RID: 4116
		// (get) Token: 0x06005D8D RID: 23949 RVA: 0x002C8B4A File Offset: 0x002C6D4A
		// (set) Token: 0x06005D8E RID: 23950 RVA: 0x002C8B52 File Offset: 0x002C6D52
		public bool IsAnimating { get; private set; }

		// Token: 0x17001015 RID: 4117
		// (get) Token: 0x06005D8F RID: 23951 RVA: 0x002C8B5B File Offset: 0x002C6D5B
		public double WorldScale
		{
			get
			{
				return this.Spherical.radius / this.config.MinDistanceFromCamera;
			}
		}

		// Token: 0x17001016 RID: 4118
		// (get) Token: 0x06005D90 RID: 23952 RVA: 0x002C8B74 File Offset: 0x002C6D74
		public Camera unityCamera
		{
			get
			{
				if (this._unityCamera == null)
				{
					this._unityCamera = Camera.main;
				}
				return this._unityCamera;
			}
		}

		// Token: 0x17001017 RID: 4119
		// (get) Token: 0x06005D91 RID: 23953 RVA: 0x002C8B95 File Offset: 0x002C6D95
		public Transform unityCameraTransform
		{
			get
			{
				if (this._unityCameraTransform == null)
				{
					this._unityCameraTransform = this._unityCamera.transform;
				}
				return this._unityCameraTransform;
			}
		}

		// Token: 0x17001018 RID: 4120
		// (get) Token: 0x06005D92 RID: 23954 RVA: 0x002C8BBC File Offset: 0x002C6DBC
		// (set) Token: 0x06005D93 RID: 23955 RVA: 0x002C8BC4 File Offset: 0x002C6DC4
		public bool ForceVisualizationUpdate { get; private set; }

		// Token: 0x06005D94 RID: 23956 RVA: 0x002C8BD0 File Offset: 0x002C6DD0
		protected override void OnStartRunning()
		{
			base.OnStartRunning();
			this.firstUpdateCompleted = false;
			this.lastObjectSelected = this.selection.SpaceObjectController;
			this.lastPosition = this.Position;
			this.usedSurfaceRotationLastFrame = this.UseSurfaceRotation;
			this.ForceVisualizationUpdate = true;
			CameraManager.Singleton = this;
		}

		// Token: 0x06005D95 RID: 23957 RVA: 0x002C8C20 File Offset: 0x002C6E20
		protected override void OnUpdate()
		{
			if (!GameControl.loadcycle100 || TIUtilities.IsInCombatMode)
			{
				return;
			}
			if (this.SelectedState == null)
			{
				this.selection.SelectObject(GameStateManager.Earth().gameObjectLink, false, false);
				return;
			}
			SpaceObjectController spaceObjectController = this.selection.SpaceObjectController;
			bool flag = ((spaceObjectController != null) ? spaceObjectController.modelLink : null) != null && (this.LOD == CameraManagerLOD.Surface || this.LOD == CameraManagerLOD.PlanetSystem) && !this.selection.SpaceObjectController.modelLink.activeSelf;
			this.ForceVisualizationUpdate = !this.firstUpdateCompleted || this.lastObjectSelected != this.selection.SpaceObjectController || flag;
			if (!this.UseSurfaceRotation && TIUtilities.IsTimeFlowing)
			{
				double num = this.targetSpherical.radius;
				double num2 = this.spherical.radius;
				if (this.lastObjectSelected != this.selection.SpaceObjectController)
				{
					num = this.lastTargetSpherical.radius;
					num2 = this.lastSpherical.radius;
				}
				Vector3d globalPositionAtTime = this.lastObjectSelected.spaceObjectState.GetGlobalPositionAtTime(TITimeState.Now());
				Vector3d vector3d = globalPositionAtTime - this.lastTargetWorldForward.normalized * num;
				this.targetSpherical = CameraManager.ComputeSphericalCoordinates(vector3d, this.FocalPoint, this.SurfaceRotation);
				Vector3d vector3d2 = globalPositionAtTime - this.lastWorldForward.normalized * num2;
				this.spherical = CameraManager.ComputeSphericalCoordinates(vector3d2, this.FocalPoint, this.SurfaceRotation);
			}
			this.HandleInput();
			this.HandleAnimations();
			this.IsAltitudeChanging = this.lastSpherical.radius != this.spherical.radius;
			if (this.IsAltitudeChanging)
			{
				this.IsAnimating = true;
			}
			this.lastObjectSelected = this.selection.SpaceObjectController;
			this.lastPosition = this.Position;
			this.lastTargetSpherical = this.targetSpherical;
			this.lastSpherical = this.spherical;
			this.lastWorldForward = this.WorldForward;
			this.lastTargetWorldForward = (Vector3)(this.FocalPoint - this.TargetPosition).normalized;
			this.lastWorldUp = ((this.UseSurfaceRotation ? this.SurfaceRotation : Quaternion.identity) * new Vector3(0f, 1f, 0f)).XZY();
			this.usedSurfaceRotationLastFrame = this.UseSurfaceRotation;
			Mood.UpdateVisualizationState(!(this.SelectedState != null) || !this.SelectedState.isEarth || this.targetSpherical.radius >= this.spherical.radius || this.LOD != CameraManagerLOD.Surface);
			this.Skybox.transform.rotation = this.SurfaceRotation * this.Transform.rotation;
			this.firstUpdateCompleted = true;
		}

		// Token: 0x06005D96 RID: 23958 RVA: 0x002C8F18 File Offset: 0x002C7118
		private void HandleInput()
		{
			if (!TIInputManager.IsMouseHoveringApplication || TIStandaloneInputModule.current.IsPointerOverUIGameObject())
			{
				return;
			}
			bool key = Input.GetKey(KeyCode.LeftControl);
			float num = Input.GetAxis("Mouse ScrollWheel");
			if (Input.GetKey(TIInputManager.cameraZoomIn))
			{
				num = 0.08f;
			}
			else if (Input.GetKey(TIInputManager.cameraZoomOut))
			{
				num = -0.08f;
			}
			if (UIMagnifier.IsMagnifierActive)
			{
				num = 0f;
			}
			if (num != 0f)
			{
				if (key)
				{
					num *= (float)this.config.ZoomRateSlow;
				}
				else
				{
					num *= (float)this.config.ZoomRateNormal;
				}
				num = Mathf.Clamp(num, -0.9f, 0.9f);
				if (num > 0f)
				{
					num = 1f - num;
				}
				else
				{
					num = 1f / (1f + num);
				}
				float num2 = 4f;
				if (this.LOD == CameraManagerLOD.SolarSystem)
				{
					float num3 = (float)Mathd.AngularDiameterOfPlane(this.OutermostSpaceBody.apoapsis_AU * 149597870700.0, this.Spherical.radius);
					if (num > 1f || num3 < 170f || this.LOD == CameraManagerLOD.PlanetSystem)
					{
						num2 = 7f;
					}
					else
					{
						num2 = 20f;
					}
				}
				num = Mathf.Pow(num, num2);
				double num4 = this.spherical.radius;
				if (this.selection.spaceObjectStateSelected != null)
				{
					double num5 = this.Spherical.radius - this.selection.spaceObjectStateSelected.meanRadius_m;
					num4 = this.selection.spaceObjectStateSelected.meanRadius_m + (double)num * num5;
				}
				else
				{
					num4 *= (double)num;
				}
				this.Zoom(num4, false);
				Mood.UpdateVisualizationState(false);
				if (GameControl.control.viewMgr.currentView == ViewType.PoliticalMap && this.LOD != CameraManagerLOD.Surface)
				{
					GameControl.control.viewMgr.currentView = ViewType.SolarSystem;
				}
			}
			float num6 = 0f;
			float num7 = 0f;
			if (Input.GetKey(TIInputManager.cameraUp))
			{
				num6 = -1f;
			}
			if (Input.GetKey(TIInputManager.cameraDown))
			{
				num6 = 1f;
			}
			if (Input.GetKey(TIInputManager.cameraLeft))
			{
				num7 = -1f;
			}
			if (Input.GetKey(TIInputManager.cameraRight))
			{
				num7 = 1f;
			}
			if (TIInputManager.IsRightMouseButtonDown)
			{
				num6 = -Input.GetAxis("Mouse Y");
				num7 = Input.GetAxis("Mouse X");
			}
			if (num6 != 0f || num7 != 0f)
			{
				float num8;
				if (key)
				{
					num8 = (float)this.config.DragRateSlow;
				}
				else
				{
					num8 = (float)this.config.DragRateNormal;
				}
				if (this.selection.HasSelection && this.selection.ObjectSelected.HasComponent<SpaceObjectController>())
				{
					SpaceObjectController component = this.selection.ObjectSelected.GetComponent<SpaceObjectController>();
					float num9 = 0.9599311f;
					float num10 = 2.0943952f;
					Vector3d position = this.Position;
					Vector3d focalPoint = this.FocalPoint;
					double num11 = Vector3d.Distance(in position, in focalPoint);
					float num12 = (float)(component.spaceObjectState.meanRadius_m / num11);
					if (num12 > 1f)
					{
						num12 = 1f;
					}
					float num13 = 2f * Mathf.Asin(num12);
					float num14 = 1f - (num13 - num9) / (num10 - num9);
					num14 = Mathf.Clamp(num14, 0.1f, 1f);
					num8 *= num14;
				}
				num6 *= num8 / 50f;
				num7 *= num8 / 50f;
				this.Rotate((double)num6, (double)num7);
				Mood.UpdateVisualizationState(false);
			}
		}

		// Token: 0x17001019 RID: 4121
		// (get) Token: 0x06005D97 RID: 23959 RVA: 0x002C926B File Offset: 0x002C746B
		private bool shouldBeginSelectionChangeAnimation
		{
			get
			{
				return this.lastObjectSelected != this.selection.SpaceObjectController;
			}
		}

		// Token: 0x1700101A RID: 4122
		// (get) Token: 0x06005D98 RID: 23960 RVA: 0x002C9283 File Offset: 0x002C7483
		private bool isSelectionChangeAnimationComplete
		{
			get
			{
				return this.selectionChangeAnimationTimeElapsed >= 1f;
			}
		}

		// Token: 0x06005D99 RID: 23961 RVA: 0x002C9298 File Offset: 0x002C7498
		private void HandleAnimations()
		{
			float num = Mathf.Min(Time.deltaTime, 0.1f);
			bool flag = false;
			this.ClampTargetRadius();
			float num2;
			if (this.LOD == CameraManagerLOD.SolarSystem || this.LOD == CameraManagerLOD.Surface)
			{
				num2 = 4.5f;
			}
			else
			{
				num2 = 2.5f;
			}
			double radius = this.TargetSpherical.radius;
			double num3 = 100.0;
			if (radius > 14959787292.918198)
			{
				num3 = 50000000.0;
			}
			else if (this.selection.spaceObjectStateSelected != null)
			{
				num3 = this.selection.spaceObjectStateSelected.meanRadius_m * 100.0 / GameStateManager.Earth().meanRadius_m;
				num3 = Mathd.Clamp(num3, 1.0, 100.0);
			}
			if (Mathd.Abs(this.spherical.radius - radius) > num3)
			{
				this.spherical.radius = Mathd.Lerp(this.spherical.radius, radius, (double)(num2 * num));
				flag = true;
			}
			else
			{
				Mood.UpdateVisualizationState(false);
			}
			SVector3d svector3d = this.TargetSpherical;
			while (Mathd.Abs(svector3d.azimuth - this.spherical.azimuth) > 3.141592653589793)
			{
				if (svector3d.azimuth > this.spherical.azimuth)
				{
					svector3d.azimuth -= 6.283185307179586;
				}
				else
				{
					svector3d.azimuth += 6.283185307179586;
				}
			}
			if (Mathd.Max(Mathd.Abs(this.spherical.polar - svector3d.polar), Mathd.Abs(this.spherical.azimuth - svector3d.azimuth)) > 0.0004363323129985824)
			{
				this.spherical.polar = Mathd.Lerp(this.spherical.polar, svector3d.polar, 3.5 * (double)num);
				this.spherical.azimuth = Mathd.Lerp(this.spherical.azimuth, svector3d.azimuth, 3.5 * (double)num);
				flag = true;
			}
			this.CachePosition();
			this.Transform.position = this.ScaledPosition(this.Position);
			if (this.shouldBeginSelectionChangeAnimation)
			{
				this.selectionChangeAnimationTimeElapsed = 0f;
				this.selectionChangeWorldForward = this.lastWorldForward;
				this.selectionChangeWorldUp = this.lastWorldUp;
			}
			this.selectionChangeAnimationTimeElapsed += num;
			if (this.isSelectionChangeAnimationComplete)
			{
				this.Transform.rotation = this.TargetLookRotation;
			}
			else
			{
				float num4 = Mathf.SmoothStep(0f, 1f, this.selectionChangeAnimationTimeElapsed / 1f);
				Quaternion quaternion = Quaternion.Inverse(this.SurfaceRotation);
				Vector3 vector = this.selectionChangeWorldForward.XZY();
				if (vector.magnitude == 0f)
				{
					vector = Vector3.up;
				}
				Vector3 vector2 = this.selectionChangeWorldUp.XZY();
				Quaternion quaternion2 = Quaternion.LookRotation(quaternion * vector, quaternion * vector2);
				this.Transform.rotation = Quaternion.Lerp(quaternion2, this.TargetLookRotation, num4);
				flag = true;
			}
			Vector3 vector3 = ((this.UseSurfaceRotation ? this.SurfaceRotation : Quaternion.identity) * new Vector3(0f, 1f, 0f)).XZY();
			if (this.usedSurfaceRotationLastFrame != this.UseSurfaceRotation || (!this.playTiltChangeAnimation && this.WorldUp != vector3))
			{
				this.tiltAnimationStartingWorldUp = this.WorldUp;
				this.tiltChangeAnimationTimeElapsed = 0f;
				this.playTiltChangeAnimation = true;
			}
			this.tiltChangeAnimationTimeElapsed += Time.deltaTime;
			if (this.playTiltChangeAnimation)
			{
				float num5 = Mathf.SmoothStep(0f, 1f, this.tiltChangeAnimationTimeElapsed / this.tiltChangeAnimationDuration);
				if (num5 >= 1f)
				{
					this.playTiltChangeAnimation = false;
				}
				this.WorldUp = Vector3.Lerp(this.tiltAnimationStartingWorldUp, vector3, num5);
				flag = true;
			}
			this.IsAnimating = flag;
		}

		// Token: 0x06005D9A RID: 23962 RVA: 0x002C9690 File Offset: 0x002C7890
		private void Transition(Vector3d transitionFocalPoint, Quaternion transitionRotation, bool setTargetSpherical, bool maintainTargetRadius)
		{
			Vector3d vector3d = this.lastPosition;
			this.Spherical = CameraManager.ComputeSphericalCoordinates(vector3d, this.FocalPoint, this.SurfaceRotation);
			if (setTargetSpherical)
			{
				double radius = this.targetSpherical.radius;
				this.TargetSpherical = this.Spherical;
				if (maintainTargetRadius)
				{
					this.targetSpherical.radius = radius;
				}
			}
		}

		// Token: 0x06005D9B RID: 23963 RVA: 0x002C96E8 File Offset: 0x002C78E8
		public void OnSelectionChanged()
		{
			Vector3d vector3d = Vector3d.zero;
			Quaternion quaternion = Quaternion.identity;
			if (this.lastObjectSelected != null)
			{
				vector3d = this.lastObjectSelected.SpaceObject.Position;
				quaternion = CameraManager.ComputeSurfaceRotation(this.lastObjectSelected);
			}
			this.Transition(vector3d, quaternion, true, false);
			this.usedSurfaceRotationLastFrame = this.UseSurfaceRotation;
		}

		// Token: 0x06005D9C RID: 23964 RVA: 0x002C9744 File Offset: 0x002C7944
		public void RotateToPolarAzimuth(double polar, double azimuth)
		{
			if (Mathd.Abs(azimuth + 6.283185307179586 - this.spherical.azimuth) < Mathd.Abs(azimuth - this.spherical.azimuth))
			{
				azimuth += 6.283185307179586;
			}
			if (Mathd.Abs(azimuth - 6.283185307179586 - this.spherical.azimuth) < Mathd.Abs(azimuth - this.spherical.azimuth))
			{
				azimuth -= 6.283185307179586;
			}
			this.targetSpherical.polar = polar;
			this.targetSpherical.azimuth = azimuth;
		}

		// Token: 0x06005D9D RID: 23965 RVA: 0x002C97E3 File Offset: 0x002C79E3
		public void RotateToLatitudeLongitude(double latitude, double longitude)
		{
			this.RotateToPolarAzimuth((90.0 - latitude) * 0.017453292519943295, longitude * 0.017453292519943295);
		}

		// Token: 0x06005D9E RID: 23966 RVA: 0x002C980C File Offset: 0x002C7A0C
		public void Rotate(double polarDelta, double azimuthDelta)
		{
			SVector3d svector3d = this.TargetSpherical;
			if (!this.UseSurfaceRotation)
			{
				svector3d = CameraManager.ComputeSphericalCoordinates(this.TargetPosition, this.FocalPoint, Quaternion.identity);
			}
			svector3d.polar += polarDelta;
			svector3d.polar = Mathd.Clamp(svector3d.polar, 0.017453292519943295, 3.12413936106985);
			svector3d.azimuth += azimuthDelta;
			SVector3d svector3d2 = svector3d;
			if (!this.UseSurfaceRotation)
			{
				svector3d2 = CameraManager.ComputeSphericalCoordinates(CameraManager.ComputePosition(svector3d2, this.FocalPoint, Quaternion.identity), this.FocalPoint, this.SurfaceRotation);
			}
			this.TargetSpherical = new SVector3d(this.TargetSpherical.radius, svector3d2.polar, svector3d2.azimuth);
		}

		// Token: 0x06005D9F RID: 23967 RVA: 0x002C98CA File Offset: 0x002C7ACA
		public void Rotate_Degrees(double polarDelta, double azimuthDelta)
		{
			this.Rotate(polarDelta * 0.017453292519943295, azimuthDelta * 0.017453292519943295);
		}

		// Token: 0x06005DA0 RID: 23968 RVA: 0x002C98E8 File Offset: 0x002C7AE8
		public void Zoom(double distance, bool isGoto = true)
		{
			if (this.targetSpherical.radius == distance)
			{
				return;
			}
			if (isGoto)
			{
				Mood.SetState(Mood.State.SDKL_Zoom);
			}
			this.targetSpherical.radius = distance;
			this.ClampTargetRadius();
		}

		// Token: 0x06005DA1 RID: 23969 RVA: 0x002C9918 File Offset: 0x002C7B18
		private void ClampTargetRadius()
		{
			double num = 2.094395160675049;
			double num2 = 0.6108652353286743;
			double num3 = 0.0;
			if (this.selection.spaceObjectStateSelected != null)
			{
				num3 = this.selection.spaceObjectStateSelected.meanRadius_m / Mathd.Sin(num / 2.0);
			}
			double num4 = 1495978707000.0;
			if (this.OutermostSpaceBody != null)
			{
				num4 = this.OutermostSpaceBody.apoapsis_AU * 149597870700.0;
			}
			double num5 = num4 / Mathd.Sin(num2 / 2.0);
			this.targetSpherical.radius = Mathd.Clamp(this.targetSpherical.radius, num3, num5);
		}

		// Token: 0x1700101B RID: 4123
		// (get) Token: 0x06005DA2 RID: 23970 RVA: 0x002C99D8 File Offset: 0x002C7BD8
		public TISpaceObjectState SelectedState
		{
			get
			{
				return this.selection.spaceObjectStateSelected;
			}
		}

		// Token: 0x1700101C RID: 4124
		// (get) Token: 0x06005DA3 RID: 23971 RVA: 0x002C99E8 File Offset: 0x002C7BE8
		private TISpaceBodyState OutermostSpaceBody
		{
			get
			{
				if (this.outermostSpaceBody == null)
				{
					IEnumerable<TISpaceBodyState> enumerable = GameStateManager.IterateByClass<TISpaceBodyState>(false);
					if (enumerable.Count<TISpaceBodyState>() > 0)
					{
						this.outermostSpaceBody = enumerable.MaxBy<TISpaceBodyState, double>((TISpaceBodyState x) => x.apoapsis_AU * 149597870700.0);
					}
				}
				return this.outermostSpaceBody;
			}
		}

		// Token: 0x1700101D RID: 4125
		// (get) Token: 0x06005DA4 RID: 23972 RVA: 0x002C9A44 File Offset: 0x002C7C44
		private float SurfaceDegrees
		{
			get
			{
				if (this.selection.spaceObjectStateSelected == null)
				{
					return 0f;
				}
				return CameraManager.GetSurfaceDegrees(this.selection.SpaceObjectController);
			}
		}

		// Token: 0x06005DA5 RID: 23973 RVA: 0x002C9A6F File Offset: 0x002C7C6F
		private static float GetSurfaceDegrees(SpaceObjectController spaceObject)
		{
			if (spaceObject == null)
			{
				return 0f;
			}
			return (float)(SpaceBodyRotating.GetSurfaceRotation(spaceObject) * 57.29577951308232);
		}

		// Token: 0x06005DA6 RID: 23974 RVA: 0x002C9A91 File Offset: 0x002C7C91
		private void CacheSurfaceRotation()
		{
			this.cachedSurfaceRotation = this.ComputeSurfaceRotation(this.SurfaceDegrees);
			this.surfaceRotationCachedFrame = TIFrameCounter.FrameCount;
			this.surfaceRotationCachedSpaceObjectController = this.selection.SpaceObjectController;
		}

		// Token: 0x1700101E RID: 4126
		// (get) Token: 0x06005DA7 RID: 23975 RVA: 0x002C9AC1 File Offset: 0x002C7CC1
		public Quaternion SurfaceRotation
		{
			get
			{
				if (this.surfaceRotationCachedFrame != TIFrameCounter.FrameCount || this.surfaceRotationCachedSpaceObjectController != this.selection.SpaceObjectController)
				{
					this.CacheSurfaceRotation();
				}
				return this.cachedSurfaceRotation;
			}
		}

		// Token: 0x06005DA8 RID: 23976 RVA: 0x002C9AF4 File Offset: 0x002C7CF4
		private static Quaternion ComputeSurfaceRotation(SpaceObjectController spaceObject, float surfaceDegrees)
		{
			if (spaceObject == null)
			{
				return Quaternion.identity;
			}
			Quaternion quaternion = (Quaternion)spaceObject.SpaceObject.SpatialRotation;
			Quaternion quaternion2 = Quaternion.AngleAxis(surfaceDegrees, Vector3.up);
			return quaternion * quaternion2;
		}

		// Token: 0x06005DA9 RID: 23977 RVA: 0x002C9B32 File Offset: 0x002C7D32
		private static Quaternion ComputeSurfaceRotation(SpaceObjectController spaceObject)
		{
			return CameraManager.ComputeSurfaceRotation(spaceObject, CameraManager.GetSurfaceDegrees(spaceObject));
		}

		// Token: 0x06005DAA RID: 23978 RVA: 0x002C9B40 File Offset: 0x002C7D40
		private Quaternion ComputeSurfaceRotation(float surfaceDegrees)
		{
			return CameraManager.ComputeSurfaceRotation(this.selection.SpaceObjectController, surfaceDegrees);
		}

		// Token: 0x06005DAB RID: 23979 RVA: 0x002C9B54 File Offset: 0x002C7D54
		private static Vector3d ComputePosition(SVector3d spherical, Vector3d focalPoint, Quaternion physicalRotation)
		{
			Vector3d xzy = (physicalRotation * new Vector3d(0f, 1f, 0f)).xzy;
			Vector3d xzy2 = (physicalRotation * new Vector3d(0f, 0f, 1f)).xzy;
			Vector3d vector3d = Vector3d.Cross(xzy, xzy2);
			Vector3d vector3d2 = Quaterniond.AngleAxis(spherical.polar * 57.29577951308232, vector3d) * xzy;
			vector3d2 = Quaterniond.AngleAxis(spherical.azimuth * 57.29577951308232, xzy) * vector3d2;
			vector3d2 *= spherical.radius;
			return focalPoint + vector3d2;
		}

		// Token: 0x06005DAC RID: 23980 RVA: 0x002C9BFF File Offset: 0x002C7DFF
		private void CachePosition()
		{
			this.cachedPosition = CameraManager.ComputePosition(this.Spherical, this.FocalPoint, this.SurfaceRotation);
		}

		// Token: 0x06005DAD RID: 23981 RVA: 0x002C9C20 File Offset: 0x002C7E20
		private static SVector3d ComputeSphericalCoordinates(Vector3d position, Vector3d focalPoint, Quaternion physicalRotation)
		{
			Vector3d xzy = (physicalRotation * new Vector3d(0f, 1f, 0f)).xzy;
			Vector3d xzy2 = (physicalRotation * new Vector3d(0f, 0f, 1f)).xzy;
			double num = Vector3d.Distance(in position, in focalPoint);
			Vector3d normalized = (position - focalPoint).normalized;
			double num2 = Vector3d.Angle(in normalized, in xzy) * 0.017453292519943295;
			Vector3d vector3d = Vector3d.Dot(in normalized, in xzy) * xzy;
			Vector3d normalized2 = (normalized - vector3d).normalized;
			double num3 = Vector3d.Angle(in normalized2, in xzy2) * 0.017453292519943295;
			Vector3d vector3d2 = Vector3d.Cross(xzy, xzy2);
			if (Vector3d.Dot(in normalized, in vector3d2) < 0.0)
			{
				num3 = 6.283185307179586 - num3;
			}
			return new SVector3d(num, num2, num3);
		}

		// Token: 0x1700101F RID: 4127
		// (get) Token: 0x06005DAE RID: 23982 RVA: 0x002C9D0F File Offset: 0x002C7F0F
		public Skybox Skybox
		{
			get
			{
				if (this._skybox == null)
				{
					this._skybox = global::UnityEngine.Object.FindObjectOfType<Skybox>();
				}
				return this._skybox;
			}
		}

		// Token: 0x06005DAF RID: 23983 RVA: 0x002C9D30 File Offset: 0x002C7F30
		public void SetSkybox(int variant)
		{
			this.Skybox.material = GameControl.assetLoader.LoadAsset<Material>(TemplateManager.global.skyboxes[variant]);
			this.skyboxBackdropPath = "skyboxes/Primary Skybox - Flat";
			this.skyboxBackdrop = GameControl.assetLoader.LoadAsset<Sprite>(this.skyboxBackdropPath);
		}

		// Token: 0x06005DB0 RID: 23984 RVA: 0x002C9D84 File Offset: 0x002C7F84
		public float ScaledDistance(double distance)
		{
			distance /= this.WorldScale;
			if (distance >= this.config.LogScaleDistanceFromCamera)
			{
				distance = this.config.LogScaleDistanceFromCamera * (Mathd.Log(distance) - Mathd.Log(this.config.LogScaleDistanceFromCamera) + 1.0);
			}
			return (float)distance;
		}

		// Token: 0x06005DB1 RID: 23985 RVA: 0x002C9DDC File Offset: 0x002C7FDC
		public float3 ScaledPosition_DoNotTouchCache(Vector3d worldPoint)
		{
			Vector3d position = this.selection.spaceObjectStateSelected.controller.SpaceObject.Position;
			Quaternion quaternion = this.ComputeSurfaceRotation(this.SurfaceDegrees);
			Vector3d vector3d = worldPoint - position;
			Vector3 vector = (Vector3)(Vector3d.Normalize(vector3d) * (double)this.ScaledDistance(Vector3d.Magnitude(in vector3d))).xzy;
			return Quaternion.Inverse(quaternion) * vector;
		}

		// Token: 0x06005DB2 RID: 23986 RVA: 0x002C9E50 File Offset: 0x002C8050
		public float3 ScaledPosition(Vector3d worldPoint, Vector3d focalPoint, Quaternion surfaceRotation)
		{
			Vector3d vector3d = worldPoint - focalPoint;
			Vector3 vector = (Vector3)(Vector3d.Normalize(vector3d) * (double)this.ScaledDistance(Vector3d.Magnitude(in vector3d))).xzy;
			return Quaternion.Inverse(surfaceRotation) * vector;
		}

		// Token: 0x06005DB3 RID: 23987 RVA: 0x002C9E9D File Offset: 0x002C809D
		public float3 ScaledPosition(Vector3d worldPoint)
		{
			return this.ScaledPosition(worldPoint, this.FocalPoint, this.SurfaceRotation);
		}

		// Token: 0x06005DB4 RID: 23988 RVA: 0x002C9EB2 File Offset: 0x002C80B2
		public float3 FastScaledPosition(Vector3d worldPoint)
		{
			return this.ScaledPosition(worldPoint, this.cachedFocalPoint, this.cachedSurfaceRotation);
		}

		// Token: 0x06005DB5 RID: 23989 RVA: 0x002C9EC8 File Offset: 0x002C80C8
		public void ScaledPositions(Orbit orbit)
		{
			SpaceObject value = orbit.Barycenter.GetComponent<SpaceObjectComponent>().Value;
			for (int i = 0; i < orbit.WorldPoints.Length; i++)
			{
				orbit.ScaledPoints[i] = this.ScaledPosition(value.Position + (value.SpatialRotation * orbit.WorldPoints[i].xzy).xzy);
			}
		}

		// Token: 0x040042DF RID: 17119
		public CameraManagerLOD LOD = CameraManagerLOD.SolarSystem;

		// Token: 0x040042E0 RID: 17120
		private Vector3d cachedFocalPoint;

		// Token: 0x040042E1 RID: 17121
		private int focalPointCachedFrame;

		// Token: 0x040042E2 RID: 17122
		private SpaceObjectController focalPointCachedSpaceObjectController;

		// Token: 0x040042E3 RID: 17123
		private SVector3d targetSpherical;

		// Token: 0x040042E4 RID: 17124
		private SVector3d lastTargetSpherical;

		// Token: 0x040042E5 RID: 17125
		private SVector3d spherical;

		// Token: 0x040042E6 RID: 17126
		private SVector3d lastSpherical;

		// Token: 0x040042E7 RID: 17127
		private Vector3 lastWorldForward;

		// Token: 0x040042E8 RID: 17128
		private Vector3 lastTargetWorldForward;

		// Token: 0x040042E9 RID: 17129
		private Vector3 lastWorldUp;

		// Token: 0x040042EB RID: 17131
		private float UpTransitionDuration = 1f;

		// Token: 0x040042EC RID: 17132
		private float UpTransitionTimeElapsed;

		// Token: 0x040042ED RID: 17133
		private Vector3 transitionUp;

		// Token: 0x040042EF RID: 17135
		private Skybox _skybox;

		// Token: 0x040042F0 RID: 17136
		public Sprite skyboxBackdrop;

		// Token: 0x040042F1 RID: 17137
		public string skyboxBackdropPath;

		// Token: 0x040042F2 RID: 17138
		private Camera _unityCamera;

		// Token: 0x040042F3 RID: 17139
		private Transform _unityCameraTransform;

		// Token: 0x040042F4 RID: 17140
		[Unity.Entities.Inject]
		private SpaceObjectSelection selection;

		// Token: 0x040042F5 RID: 17141
		[global::Zenject.Inject]
		public CameraConfig config;

		// Token: 0x040042F6 RID: 17142
		public static CameraManager Singleton;

		// Token: 0x040042F7 RID: 17143
		private SpaceObjectController lastObjectSelected;

		// Token: 0x040042F8 RID: 17144
		private Vector3d lastPosition;

		// Token: 0x040042F9 RID: 17145
		private bool usedSurfaceRotationLastFrame;

		// Token: 0x040042FA RID: 17146
		private bool firstUpdateCompleted;

		// Token: 0x040042FC RID: 17148
		private const float selectionChangeAnimationDuration = 1f;

		// Token: 0x040042FD RID: 17149
		private float selectionChangeAnimationTimeElapsed;

		// Token: 0x040042FE RID: 17150
		private Vector3 selectionChangeWorldForward;

		// Token: 0x040042FF RID: 17151
		private Vector3 selectionChangeWorldUp;

		// Token: 0x04004300 RID: 17152
		private float tiltChangeAnimationDuration = 1f;

		// Token: 0x04004301 RID: 17153
		private float tiltChangeAnimationTimeElapsed;

		// Token: 0x04004302 RID: 17154
		private bool playTiltChangeAnimation;

		// Token: 0x04004303 RID: 17155
		private Vector3 tiltAnimationStartingWorldUp;

		// Token: 0x04004304 RID: 17156
		public bool IsAltitudeChanging;

		// Token: 0x04004305 RID: 17157
		private TISpaceBodyState outermostSpaceBody;

		// Token: 0x04004306 RID: 17158
		private Quaternion cachedSurfaceRotation;

		// Token: 0x04004307 RID: 17159
		private int surfaceRotationCachedFrame;

		// Token: 0x04004308 RID: 17160
		private SpaceObjectController surfaceRotationCachedSpaceObjectController;

		// Token: 0x04004309 RID: 17161
		private Vector3d cachedPosition;
	}
}
