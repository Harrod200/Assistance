using System;
using ModelShark;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008FD RID: 2301
	public class UIMagnifier : MonoBehaviour
	{
		// Token: 0x06005811 RID: 22545 RVA: 0x0028663A File Offset: 0x0028483A
		private void Awake()
		{
			this.lensRect = base.GetComponent<RectTransform>();
			this.magnifierCanvas = base.GetComponentInParent<Canvas>();
			if (this.magnifierCanvas == null)
			{
				Debug.LogError("UIMagnifier: No parent Canvas found. Magnifier must be inside a Canvas.");
			}
		}

		// Token: 0x06005812 RID: 22546 RVA: 0x0028666C File Offset: 0x0028486C
		private void OnEnable()
		{
			this.EnsureRenderTexture();
			this.AssignTexture();
			this.targetZoom = this.zoom;
			this.isHotkeyHeld = false;
			this.magnifierCanvas.enabled = false;
		}

		// Token: 0x06005813 RID: 22547 RVA: 0x00286699 File Offset: 0x00284899
		private void OnDisable()
		{
			if (this.screenRT != null)
			{
				this.screenRT.Release();
				this.screenRT = null;
			}
		}

		// Token: 0x06005814 RID: 22548 RVA: 0x002866BC File Offset: 0x002848BC
		private void Update()
		{
			if (SceneManager.GetActiveScene().name != "StartScreenScene" && !GameControl.loadcycle100)
			{
				return;
			}
			if (!TIPlayerProfileManager.enableAccessibilityMagnifier)
			{
				return;
			}
			if (this.magnifierCanvas == null)
			{
				return;
			}
			bool flag = TIInputManager.IsHotkeyTriggered(TIInputManager.AccessibilityMagnifier, TIInputManager.KeyPressMode.Continous);
			this.EnsureRenderTexture();
			this.AssignTexture();
			if (!this.isHotkeyHeld && flag)
			{
				this.isHotkeyHeld = true;
				this.hasCapturedThisHold = false;
				UIMagnifier.IsMagnifierActive = true;
				TooltipManager.Instance.HideAll();
				TooltipManager.Instance.tooltipsEnabled = false;
			}
			else if (this.isHotkeyHeld && !flag)
			{
				this.isHotkeyHeld = false;
				this.magnifierCanvas.enabled = false;
				UIMagnifier.IsMagnifierActive = false;
				TooltipManager.Instance.tooltipsEnabled = true;
			}
			if (this.isHotkeyHeld && !this.hasCapturedThisHold)
			{
				this.CaptureScreen();
				this.hasCapturedThisHold = true;
				Vector2 vector = new Vector2(this.magnifiedImage.rectTransform.sizeDelta.y * TIUtilities.GetScreenRatio(), this.magnifiedImage.rectTransform.sizeDelta.y);
				this.magnifiedImage.rectTransform.sizeDelta = vector;
				this.lensRect.sizeDelta = vector;
				this.blockerRect.sizeDelta = vector;
				this.magnifierCanvas.enabled = true;
			}
			if (this.isHotkeyHeld)
			{
				if (TIInputManager.IsMouseHoveringApplication)
				{
					float num = Input.mouseScrollDelta.y;
					if (Input.GetKey(TIInputManager.cameraZoomIn))
					{
						num += this.zoomStep;
					}
					if (Input.GetKey(TIInputManager.cameraZoomOut))
					{
						num -= this.zoomStep;
					}
					if (num != 0f)
					{
						this.targetZoom += num * this.zoomStep;
						this.targetZoom = Mathf.Clamp(this.targetZoom, this.minZoom, this.maxZoom);
					}
				}
				this.zoom = Mathf.SmoothDamp(this.zoom, this.targetZoom, ref this.zoomVelocity, this.zoomSmoothTime);
				this.UpdateLensAndImage();
			}
		}

		// Token: 0x06005815 RID: 22549 RVA: 0x002868B8 File Offset: 0x00284AB8
		private void EnsureRenderTexture()
		{
			int width = Screen.width;
			int height = Screen.height;
			if (this.screenRT != null && (this.screenRT.width != width || this.screenRT.height != height))
			{
				this.screenRT.Release();
				this.screenRT = null;
			}
			if (this.screenRT == null)
			{
				this.screenRT = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
				this.screenRT.name = "TI_UI_Magnifier_RT";
				this.screenRT.useMipMap = false;
				this.screenRT.autoGenerateMips = false;
			}
		}

		// Token: 0x06005816 RID: 22550 RVA: 0x00286952 File Offset: 0x00284B52
		private void AssignTexture()
		{
			if (this.magnifiedImage != null && this.magnifiedImage.texture != this.screenRT)
			{
				this.magnifiedImage.texture = this.screenRT;
			}
		}

		// Token: 0x06005817 RID: 22551 RVA: 0x0028698B File Offset: 0x00284B8B
		private void CaptureScreen()
		{
			if (this.screenRT == null)
			{
				return;
			}
			ScreenCapture.CaptureScreenshotIntoRenderTexture(this.screenRT);
		}

		// Token: 0x06005818 RID: 22552 RVA: 0x002869A8 File Offset: 0x00284BA8
		private void UpdateLensAndImage()
		{
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.magnifierCanvas.transform as RectTransform, Input.mousePosition, null, out vector);
			RectTransform rectTransform = this.lensRect.root as RectTransform;
			Vector2 vector2 = this.lensRect.rect.size * 0.5f;
			Vector2 vector3 = rectTransform.rect.size * 0.5f;
			float num = Mathf.Clamp(vector.x, -vector3.x + vector2.x, vector3.x - vector2.x);
			float num2 = Mathf.Clamp(vector.y, -vector3.y + vector2.y, vector3.y - vector2.y);
			this.lensRect.anchoredPosition = new Vector2(num, num2);
			this.magnifiedImage.rectTransform.localScale = new Vector3(1f, -1f, 1f);
			this.uvX = Input.mousePosition.x / (float)Screen.width;
			this.uvY = 1f - Input.mousePosition.y / (float)Screen.height;
			this.baseUVSize = this.lensRect.rect.width / (float)Screen.width;
			this.uvSize = this.baseUVSize / (this.zoom * (1080f / (float)Screen.height));
			this.uvHalf = this.uvSize * 0.5f;
			this.uvX = Mathf.Clamp(this.uvX, this.uvHalf, 1f - this.uvHalf);
			this.uvY = Mathf.Clamp(this.uvY, this.uvHalf, 1f - this.uvHalf);
			this.magnifiedImage.uvRect = new Rect(this.uvX - this.uvHalf, this.uvY - this.uvHalf, this.uvSize, this.uvSize);
		}

		// Token: 0x04003F9A RID: 16282
		[Header("Magnifier Objects")]
		public RawImage magnifiedImage;

		// Token: 0x04003F9B RID: 16283
		private Canvas magnifierCanvas;

		// Token: 0x04003F9C RID: 16284
		public RectTransform blockerRect;

		// Token: 0x04003F9D RID: 16285
		private RectTransform lensRect;

		// Token: 0x04003F9E RID: 16286
		private RenderTexture screenRT;

		// Token: 0x04003F9F RID: 16287
		[Header("Zoom")]
		[Tooltip("Current zoom level. 1 = show area under lens.")]
		[Min(1f)]
		public float zoom = 1.5f;

		// Token: 0x04003FA0 RID: 16288
		public float minZoom = 1f;

		// Token: 0x04003FA1 RID: 16289
		public float maxZoom = 5f;

		// Token: 0x04003FA2 RID: 16290
		public float zoomStep = 0.15f;

		// Token: 0x04003FA3 RID: 16291
		[Header("Smooth Zooming")]
		public float zoomSmoothTime = 0.025f;

		// Token: 0x04003FA4 RID: 16292
		private float targetZoom;

		// Token: 0x04003FA5 RID: 16293
		private float zoomVelocity;

		// Token: 0x04003FA6 RID: 16294
		private bool isHotkeyHeld;

		// Token: 0x04003FA7 RID: 16295
		private bool hasCapturedThisHold;

		// Token: 0x04003FA8 RID: 16296
		public static bool IsMagnifierActive;

		// Token: 0x04003FA9 RID: 16297
		private float uvX;

		// Token: 0x04003FAA RID: 16298
		private float uvY;

		// Token: 0x04003FAB RID: 16299
		private float baseUVSize;

		// Token: 0x04003FAC RID: 16300
		private float uvSize;

		// Token: 0x04003FAD RID: 16301
		private float uvHalf;
	}
}
