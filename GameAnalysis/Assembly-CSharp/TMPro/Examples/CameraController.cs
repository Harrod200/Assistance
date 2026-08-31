using System;
using UnityEngine;

namespace TMPro.Examples
{
	// Token: 0x02000501 RID: 1281
	[Obsolete]
	public class CameraController : MonoBehaviour
	{
		// Token: 0x06001FB6 RID: 8118 RVA: 0x000A455C File Offset: 0x000A275C
		private void Awake()
		{
			if (QualitySettings.vSyncCount > 0)
			{
				Application.targetFrameRate = 60;
			}
			else
			{
				Application.targetFrameRate = -1;
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
			{
				Input.simulateMouseWithTouches = false;
			}
			this.cameraTransform = base.transform;
			this.previousSmoothing = this.MovementSmoothing;
			this.mainCamera = Camera.main;
		}

		// Token: 0x06001FB7 RID: 8119 RVA: 0x000A45BA File Offset: 0x000A27BA
		private void Start()
		{
			if (this.CameraTarget == null)
			{
				this.dummyTarget = new GameObject("Camera Target").transform;
				this.CameraTarget = this.dummyTarget;
			}
		}

		// Token: 0x06001FB8 RID: 8120 RVA: 0x000A45EC File Offset: 0x000A27EC
		private void LateUpdate()
		{
			this.GetPlayerInput();
			if (this.CameraTarget != null)
			{
				if (this.CameraMode == CameraController.CameraModes.Isometric)
				{
					this.desiredPosition = this.CameraTarget.position + Quaternion.Euler(this.ElevationAngle, this.OrbitalAngle, 0f) * new Vector3(0f, 0f, -this.FollowDistance);
				}
				else if (this.CameraMode == CameraController.CameraModes.Follow)
				{
					this.desiredPosition = this.CameraTarget.position + this.CameraTarget.TransformDirection(Quaternion.Euler(this.ElevationAngle, this.OrbitalAngle, 0f) * new Vector3(0f, 0f, -this.FollowDistance));
				}
				if (this.MovementSmoothing)
				{
					this.cameraTransform.position = Vector3.SmoothDamp(this.cameraTransform.position, this.desiredPosition, ref this.currentVelocity, this.MovementSmoothingValue * Time.fixedDeltaTime);
				}
				else
				{
					this.cameraTransform.position = this.desiredPosition;
				}
				if (this.RotationSmoothing)
				{
					this.cameraTransform.rotation = Quaternion.Lerp(this.cameraTransform.rotation, Quaternion.LookRotation(this.CameraTarget.position - this.cameraTransform.position), this.RotationSmoothingValue * Time.deltaTime);
					return;
				}
				this.cameraTransform.LookAt(this.CameraTarget);
			}
		}

		// Token: 0x06001FB9 RID: 8121 RVA: 0x000A476C File Offset: 0x000A296C
		private void GetPlayerInput()
		{
			this.moveVector = Vector3.zero;
			this.mouseWheel = Input.GetAxis("Mouse ScrollWheel");
			float num = (float)Input.touchCount;
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || num > 0f)
			{
				this.mouseWheel *= 10f;
				if (Input.GetKeyDown(KeyCode.I))
				{
					this.CameraMode = CameraController.CameraModes.Isometric;
				}
				if (Input.GetKeyDown(KeyCode.F))
				{
					this.CameraMode = CameraController.CameraModes.Follow;
				}
				if (Input.GetKeyDown(KeyCode.S))
				{
					this.MovementSmoothing = !this.MovementSmoothing;
				}
				if (Input.GetMouseButton(1))
				{
					this.mouseY = Input.GetAxis("Mouse Y");
					this.mouseX = Input.GetAxis("Mouse X");
					if (this.mouseY > 0.01f || this.mouseY < -0.01f)
					{
						this.ElevationAngle -= this.mouseY * this.MoveSensitivity;
						this.ElevationAngle = Mathf.Clamp(this.ElevationAngle, this.MinElevationAngle, this.MaxElevationAngle);
					}
					if (this.mouseX > 0.01f || this.mouseX < -0.01f)
					{
						this.OrbitalAngle += this.mouseX * this.MoveSensitivity;
						if (this.OrbitalAngle > 360f)
						{
							this.OrbitalAngle -= 360f;
						}
						if (this.OrbitalAngle < 0f)
						{
							this.OrbitalAngle += 360f;
						}
					}
				}
				if (num == 1f && Input.GetTouch(0).phase == TouchPhase.Moved)
				{
					Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;
					if (deltaPosition.y > 0.01f || deltaPosition.y < -0.01f)
					{
						this.ElevationAngle -= deltaPosition.y * 0.1f;
						this.ElevationAngle = Mathf.Clamp(this.ElevationAngle, this.MinElevationAngle, this.MaxElevationAngle);
					}
					if (deltaPosition.x > 0.01f || deltaPosition.x < -0.01f)
					{
						this.OrbitalAngle += deltaPosition.x * 0.1f;
						if (this.OrbitalAngle > 360f)
						{
							this.OrbitalAngle -= 360f;
						}
						if (this.OrbitalAngle < 0f)
						{
							this.OrbitalAngle += 360f;
						}
					}
				}
				RaycastHit raycastHit;
				if (Input.GetMouseButton(0) && Physics.Raycast(this.mainCamera.ScreenPointToRay(Input.mousePosition), out raycastHit, 300f, 23552))
				{
					if (raycastHit.transform == this.CameraTarget)
					{
						this.OrbitalAngle = 0f;
					}
					else
					{
						this.CameraTarget = raycastHit.transform;
						this.OrbitalAngle = 0f;
						this.MovementSmoothing = this.previousSmoothing;
					}
				}
				if (Input.GetMouseButton(2))
				{
					if (this.dummyTarget == null)
					{
						this.dummyTarget = new GameObject("Camera Target").transform;
						this.dummyTarget.position = this.CameraTarget.position;
						this.dummyTarget.rotation = this.CameraTarget.rotation;
						this.CameraTarget = this.dummyTarget;
						this.previousSmoothing = this.MovementSmoothing;
						this.MovementSmoothing = false;
					}
					else if (this.dummyTarget != this.CameraTarget)
					{
						this.dummyTarget.position = this.CameraTarget.position;
						this.dummyTarget.rotation = this.CameraTarget.rotation;
						this.CameraTarget = this.dummyTarget;
						this.previousSmoothing = this.MovementSmoothing;
						this.MovementSmoothing = false;
					}
					this.mouseY = Input.GetAxis("Mouse Y");
					this.mouseX = Input.GetAxis("Mouse X");
					this.moveVector = this.cameraTransform.TransformDirection(this.mouseX, this.mouseY, 0f);
					this.dummyTarget.Translate(-this.moveVector, Space.World);
				}
			}
			if (num == 2f)
			{
				Touch touch = Input.GetTouch(0);
				Touch touch2 = Input.GetTouch(1);
				Vector2 vector = touch.position - touch.deltaPosition;
				Vector2 vector2 = touch2.position - touch2.deltaPosition;
				float magnitude = (vector - vector2).magnitude;
				float magnitude2 = (touch.position - touch2.position).magnitude;
				float num2 = magnitude - magnitude2;
				if (num2 > 0.01f || num2 < -0.01f)
				{
					this.FollowDistance += num2 * 0.25f;
					this.FollowDistance = Mathf.Clamp(this.FollowDistance, this.MinFollowDistance, this.MaxFollowDistance);
				}
			}
			if (this.mouseWheel < -0.01f || this.mouseWheel > 0.01f)
			{
				this.FollowDistance -= this.mouseWheel * 5f;
				this.FollowDistance = Mathf.Clamp(this.FollowDistance, this.MinFollowDistance, this.MaxFollowDistance);
			}
		}

		// Token: 0x0400184E RID: 6222
		private Transform cameraTransform;

		// Token: 0x0400184F RID: 6223
		private Transform dummyTarget;

		// Token: 0x04001850 RID: 6224
		public Transform CameraTarget;

		// Token: 0x04001851 RID: 6225
		public float FollowDistance = 30f;

		// Token: 0x04001852 RID: 6226
		public float MaxFollowDistance = 100f;

		// Token: 0x04001853 RID: 6227
		public float MinFollowDistance = 2f;

		// Token: 0x04001854 RID: 6228
		public float ElevationAngle = 30f;

		// Token: 0x04001855 RID: 6229
		public float MaxElevationAngle = 85f;

		// Token: 0x04001856 RID: 6230
		public float MinElevationAngle;

		// Token: 0x04001857 RID: 6231
		public float OrbitalAngle;

		// Token: 0x04001858 RID: 6232
		public CameraController.CameraModes CameraMode;

		// Token: 0x04001859 RID: 6233
		public bool MovementSmoothing = true;

		// Token: 0x0400185A RID: 6234
		public bool RotationSmoothing;

		// Token: 0x0400185B RID: 6235
		private bool previousSmoothing;

		// Token: 0x0400185C RID: 6236
		public float MovementSmoothingValue = 25f;

		// Token: 0x0400185D RID: 6237
		public float RotationSmoothingValue = 5f;

		// Token: 0x0400185E RID: 6238
		public float MoveSensitivity = 2f;

		// Token: 0x0400185F RID: 6239
		private Vector3 currentVelocity = Vector3.zero;

		// Token: 0x04001860 RID: 6240
		private Vector3 desiredPosition;

		// Token: 0x04001861 RID: 6241
		private float mouseX;

		// Token: 0x04001862 RID: 6242
		private float mouseY;

		// Token: 0x04001863 RID: 6243
		private Vector3 moveVector;

		// Token: 0x04001864 RID: 6244
		private float mouseWheel;

		// Token: 0x04001865 RID: 6245
		private Camera mainCamera;

		// Token: 0x04001866 RID: 6246
		private const string event_SmoothingValue = "Slider - Smoothing Value";

		// Token: 0x04001867 RID: 6247
		private const string event_FollowDistance = "Slider - Camera Zoom";

		// Token: 0x02000C76 RID: 3190
		public enum CameraModes
		{
			// Token: 0x04004E72 RID: 20082
			Follow,
			// Token: 0x04004E73 RID: 20083
			Isometric,
			// Token: 0x04004E74 RID: 20084
			Free
		}
	}
}
