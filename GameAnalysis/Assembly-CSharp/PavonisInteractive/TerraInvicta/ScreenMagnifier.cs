using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008FC RID: 2300
	[ExecuteInEditMode]
	public class ScreenMagnifier : MonoBehaviour
	{
		// Token: 0x0600580A RID: 22538 RVA: 0x002862E8 File Offset: 0x002844E8
		private void OnValidate()
		{
			this.material.SetColor("_BorderColor", this.borderColor);
			this.material.SetFloat("_BorderThicknessPixels", this.borderThicknessPixels);
			this.material.SetFloat("_Zoom", this.zoom);
		}

		// Token: 0x0600580B RID: 22539 RVA: 0x00286337 File Offset: 0x00284537
		private void OnEnable()
		{
		}

		// Token: 0x0600580C RID: 22540 RVA: 0x00286339 File Offset: 0x00284539
		private void OnDisable()
		{
		}

		// Token: 0x0600580D RID: 22541 RVA: 0x0028633C File Offset: 0x0028453C
		private void StartMagnify()
		{
			if (this.material == null)
			{
				return;
			}
			this.material.SetColor("_BorderColor", this.borderColor);
			this.material.SetFloat("_BorderThicknessPixels", this.borderThicknessPixels);
			this.cam = base.GetComponent<Camera>();
			this.capturedRT = Shader.PropertyToID("_CapturedFinalImage");
			this.cbCapture = new CommandBuffer();
			this.cbCapture.name = "Capture Final Post-Processed Image";
			this.cbCapture.GetTemporaryRT(this.capturedRT, -1, -1, 0, FilterMode.Bilinear);
			this.cbCapture.Blit(BuiltinRenderTextureType.CameraTarget, this.capturedRT);
			this.cam.AddCommandBuffer(CameraEvent.AfterImageEffects, this.cbCapture);
			this.cbFinal = new CommandBuffer();
			this.cbFinal.name = "Final Screen Magnifier";
			this.cbFinal.Blit(this.capturedRT, BuiltinRenderTextureType.CameraTarget, this.material);
			this.cam.AddCommandBuffer(CameraEvent.AfterEverything, this.cbFinal);
			this.magnifying = true;
		}

		// Token: 0x0600580E RID: 22542 RVA: 0x00286458 File Offset: 0x00284658
		private void StopMagnify()
		{
			this.magnifying = false;
			if (this.cam != null)
			{
				if (this.cbCapture != null)
				{
					this.cam.RemoveCommandBuffer(CameraEvent.AfterImageEffects, this.cbCapture);
				}
				if (this.cbFinal != null)
				{
					this.cam.RemoveCommandBuffer(CameraEvent.AfterEverything, this.cbFinal);
				}
			}
		}

		// Token: 0x0600580F RID: 22543 RVA: 0x002864B0 File Offset: 0x002846B0
		private void Update()
		{
			if (this.material == null)
			{
				return;
			}
			Vector3 mousePosition = Input.mousePosition;
			float num = mousePosition.x / (float)Screen.width;
			float num2 = mousePosition.y / (float)Screen.height;
			Vector2 vector = new Vector2(num, num2);
			Vector2 vector2 = this.boxSizeUV * 0.5f;
			float num3 = Mathf.Clamp01(vector.x - vector2.x);
			float num4 = Mathf.Clamp01(vector.y - vector2.y);
			float num5 = Mathf.Clamp01(vector.x + vector2.x);
			float num6 = Mathf.Clamp01(vector.y + vector2.y);
			this.material.SetVector("_Bounds", new Vector4(num3, num4, num5, num6));
			this.zoom = Mathf.Clamp(this.zoom + Input.GetAxis("Mouse ScrollWheel"), this.zoomBounds.x, this.zoomBounds.y);
			this.material.SetFloat("_Zoom", this.zoom);
			if (TIInputManager.IsControlKeyDown)
			{
				if (!this.magnifying)
				{
					this.StartMagnify();
					return;
				}
			}
			else if (this.magnifying)
			{
				this.StopMagnify();
			}
		}

		// Token: 0x04003F8F RID: 16271
		public Material material;

		// Token: 0x04003F90 RID: 16272
		[Header("Magnifier Box")]
		public Vector2 boxSizeUV = new Vector2(0.2f, 0.2f);

		// Token: 0x04003F91 RID: 16273
		public float zoom = 2f;

		// Token: 0x04003F92 RID: 16274
		public Vector2 zoomBounds = new Vector2(1.25f, 5f);

		// Token: 0x04003F93 RID: 16275
		public Color borderColor = Color.white;

		// Token: 0x04003F94 RID: 16276
		public float borderThicknessPixels = 3f;

		// Token: 0x04003F95 RID: 16277
		private Camera cam;

		// Token: 0x04003F96 RID: 16278
		private CommandBuffer cbCapture;

		// Token: 0x04003F97 RID: 16279
		private CommandBuffer cbFinal;

		// Token: 0x04003F98 RID: 16280
		private int capturedRT;

		// Token: 0x04003F99 RID: 16281
		private bool magnifying;
	}
}
