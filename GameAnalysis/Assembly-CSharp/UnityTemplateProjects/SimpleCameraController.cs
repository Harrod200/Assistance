using System;
using UnityEngine;

namespace UnityTemplateProjects
{
	// Token: 0x02000515 RID: 1301
	public class SimpleCameraController : MonoBehaviour
	{
		// Token: 0x06002019 RID: 8217 RVA: 0x000A6A3B File Offset: 0x000A4C3B
		private void OnEnable()
		{
			this.m_TargetCameraState.SetFromTransform(base.transform);
			this.m_InterpolatingCameraState.SetFromTransform(base.transform);
		}

		// Token: 0x0600201A RID: 8218 RVA: 0x000A6A60 File Offset: 0x000A4C60
		private Vector3 GetInputTranslationDirection()
		{
			Vector3 vector = default(Vector3);
			if (Input.GetKey(TIInputManager.cameraUp))
			{
				vector += Vector3.forward;
			}
			if (Input.GetKey(TIInputManager.cameraDown))
			{
				vector += Vector3.back;
			}
			if (Input.GetKey(TIInputManager.cameraLeft))
			{
				vector += Vector3.left;
			}
			if (Input.GetKey(TIInputManager.cameraRight))
			{
				vector += Vector3.right;
			}
			if (Input.GetKey(KeyCode.Q))
			{
				vector += Vector3.down;
			}
			if (Input.GetKey(KeyCode.E))
			{
				vector += Vector3.up;
			}
			return vector;
		}

		// Token: 0x0600201B RID: 8219 RVA: 0x000A6B00 File Offset: 0x000A4D00
		private void Update()
		{
			if (Input.GetMouseButtonDown(1))
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
			if (Input.GetMouseButtonUp(1))
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
			if (Input.GetMouseButton(1))
			{
				Vector2 vector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (float)(this.invertY ? 1 : (-1)));
				float num = this.mouseSensitivityCurve.Evaluate(vector.magnitude);
				this.m_TargetCameraState.yaw += vector.x * num;
				this.m_TargetCameraState.pitch += vector.y * num;
			}
			Vector3 vector2 = this.GetInputTranslationDirection() * Time.deltaTime;
			if (Input.GetKey(KeyCode.LeftShift))
			{
				vector2 *= 10f;
			}
			this.boost += Input.mouseScrollDelta.y * 0.2f;
			vector2 *= Mathf.Pow(2f, this.boost);
			this.m_TargetCameraState.Translate(vector2);
			float num2 = 1f - Mathf.Exp(Mathf.Log(0.00999999f) / this.positionLerpTime * Time.deltaTime);
			float num3 = 1f - Mathf.Exp(Mathf.Log(0.00999999f) / this.rotationLerpTime * Time.deltaTime);
			this.m_InterpolatingCameraState.LerpTowards(this.m_TargetCameraState, num2, num3);
			this.m_InterpolatingCameraState.UpdateTransform(base.transform);
		}

		// Token: 0x040018CB RID: 6347
		private SimpleCameraController.CameraState m_TargetCameraState = new SimpleCameraController.CameraState();

		// Token: 0x040018CC RID: 6348
		private SimpleCameraController.CameraState m_InterpolatingCameraState = new SimpleCameraController.CameraState();

		// Token: 0x040018CD RID: 6349
		[Header("Movement Settings")]
		[Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
		public float boost = 3.5f;

		// Token: 0x040018CE RID: 6350
		[Tooltip("Time it takes to interpolate camera position 99% of the way to the target.")]
		[Range(0.001f, 1f)]
		public float positionLerpTime = 0.2f;

		// Token: 0x040018CF RID: 6351
		[Header("Rotation Settings")]
		[Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
		public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0.5f, 0f, 5f),
			new Keyframe(1f, 2.5f, 0f, 0f)
		});

		// Token: 0x040018D0 RID: 6352
		[Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target.")]
		[Range(0.001f, 1f)]
		public float rotationLerpTime = 0.01f;

		// Token: 0x040018D1 RID: 6353
		[Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
		public bool invertY;

		// Token: 0x02000C89 RID: 3209
		private class CameraState
		{
			// Token: 0x06006D15 RID: 27925 RVA: 0x0030A3D4 File Offset: 0x003085D4
			public void SetFromTransform(Transform t)
			{
				this.pitch = t.eulerAngles.x;
				this.yaw = t.eulerAngles.y;
				this.roll = t.eulerAngles.z;
				this.x = t.position.x;
				this.y = t.position.y;
				this.z = t.position.z;
			}

			// Token: 0x06006D16 RID: 27926 RVA: 0x0030A448 File Offset: 0x00308648
			public void Translate(Vector3 translation)
			{
				Vector3 vector = Quaternion.Euler(this.pitch, this.yaw, this.roll) * translation;
				this.x += vector.x;
				this.y += vector.y;
				this.z += vector.z;
			}

			// Token: 0x06006D17 RID: 27927 RVA: 0x0030A4AC File Offset: 0x003086AC
			public void LerpTowards(SimpleCameraController.CameraState target, float positionLerpPct, float rotationLerpPct)
			{
				this.yaw = Mathf.Lerp(this.yaw, target.yaw, rotationLerpPct);
				this.pitch = Mathf.Lerp(this.pitch, target.pitch, rotationLerpPct);
				this.roll = Mathf.Lerp(this.roll, target.roll, rotationLerpPct);
				this.x = Mathf.Lerp(this.x, target.x, positionLerpPct);
				this.y = Mathf.Lerp(this.y, target.y, positionLerpPct);
				this.z = Mathf.Lerp(this.z, target.z, positionLerpPct);
			}

			// Token: 0x06006D18 RID: 27928 RVA: 0x0030A549 File Offset: 0x00308749
			public void UpdateTransform(Transform t)
			{
				t.eulerAngles = new Vector3(this.pitch, this.yaw, this.roll);
				t.position = new Vector3(this.x, this.y, this.z);
			}

			// Token: 0x04004ED9 RID: 20185
			public float yaw;

			// Token: 0x04004EDA RID: 20186
			public float pitch;

			// Token: 0x04004EDB RID: 20187
			public float roll;

			// Token: 0x04004EDC RID: 20188
			public float x;

			// Token: 0x04004EDD RID: 20189
			public float y;

			// Token: 0x04004EDE RID: 20190
			public float z;
		}
	}
}
