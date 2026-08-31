using System;
using System.Collections;
using System.Collections.Generic;
using ModelShark;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000448 RID: 1096
public class TutorialTip : MonoBehaviour
{
	// Token: 0x1700033A RID: 826
	// (get) Token: 0x060016E6 RID: 5862 RVA: 0x000759C0 File Offset: 0x00073BC0
	public static TutorialTip Instance
	{
		get
		{
			if (TutorialTip._instance == null)
			{
				GameObject gameObject = GameControl.assetLoader.InstantiatePrefab("ui/TutorialTipCanvas");
				gameObject.name = "Tutorial Tip (Single)";
				gameObject.transform.SetAsFirstSibling();
				TutorialTip._instance = gameObject.GetComponentInChildren<TutorialTip>();
				TutorialTip._instance.SetCanvasScaling();
			}
			return TutorialTip._instance;
		}
	}

	// Token: 0x1700033B RID: 827
	// (get) Token: 0x060016E7 RID: 5863 RVA: 0x00075A18 File Offset: 0x00073C18
	public static bool InstanceNull
	{
		get
		{
			return TutorialTip._instance == null;
		}
	}

	// Token: 0x1700033C RID: 828
	// (get) Token: 0x060016E8 RID: 5864 RVA: 0x00075A25 File Offset: 0x00073C25
	public static bool TipVisible
	{
		get
		{
			return TutorialTip._instance != null && TutorialTip._instance.gameObject.activeSelf;
		}
	}

	// Token: 0x060016E9 RID: 5865 RVA: 0x00075A48 File Offset: 0x00073C48
	private void Awake()
	{
		this.tipCanvas = base.gameObject.GetComponentInParent<Canvas>();
		this.tipCanvas.worldCamera = Camera.main;
		this.tipCanvasRT = this.tipCanvas.transform as RectTransform;
		this.tipCanvasRT.pivot = new Vector2(0f, 1f);
		this.UpdateUIScaling();
		GameControl.eventManager.AddListener<UIScaleSettingChange>(new EventManager.EventDelegate<UIScaleSettingChange>(this.OnUIScaleChanged), null, null, true, false);
	}

	// Token: 0x060016EA RID: 5866 RVA: 0x00075AC8 File Offset: 0x00073CC8
	private void Update()
	{
		if (this.parentController != null && this.parentController.is3D && this.Target3D != null)
		{
			base.transform.position = this.mainCamera.WorldToScreenPoint(this.Target3D.transform.position);
			base.transform.localPosition += new Vector3(0f, this.rectTransform.sizeDelta.y + 50f, 0f);
			if (this.arrowRect3D.gameObject.activeSelf)
			{
				this.arrowRect3D.gameObject.SetActive(false);
			}
			if (!this.haloRect3D.gameObject.activeSelf)
			{
				this.haloRect3D.gameObject.SetActive(true);
			}
			this.haloRect3D.transform.position = this.mainCamera.WorldToScreenPoint(this.Target3D.transform.position);
			this.tutorialPointer2DContainer.anchoredPosition = (base.transform.localPosition + this.tutorialPointerContainer.localPosition + Vector3.down * 80f) * -1f;
			return;
		}
		if (this.haloRect3D.gameObject.activeSelf)
		{
			this.haloRect3D.gameObject.SetActive(false);
		}
		this.tutorialPointer2DContainer.anchoredPosition = Vector2.zero;
	}

	// Token: 0x060016EB RID: 5867 RVA: 0x00075C58 File Offset: 0x00073E58
	public void SetupTip(string locName, string locDesc, GameObject targetObject, UITutorialActionType tutorialAction, bool disableHighlightBlocker, bool nextFrame = false, TutorialTip.ArrowDirection arrowDirOverride = TutorialTip.ArrowDirection.None, GameObject fallbackTargetObject = null, Sprite tutorialImage = null, List<int> controlIDs = null, GameObject targetObject3D = null)
	{
		base.StartCoroutine(this.SetupTipWithDelay(locName, locDesc, targetObject, tutorialAction, disableHighlightBlocker, nextFrame, arrowDirOverride, fallbackTargetObject, tutorialImage, controlIDs, targetObject3D));
	}

	// Token: 0x060016EC RID: 5868 RVA: 0x00075C88 File Offset: 0x00073E88
	private IEnumerator SetupTipWithDelay(string locName, string locDesc, GameObject targetObject, UITutorialActionType tutorialAction, bool disableHighlightBlocker, bool nextFrame = false, TutorialTip.ArrowDirection arrowDirOverride = TutorialTip.ArrowDirection.None, GameObject fallbackTargetObject = null, Sprite tutorialImage = null, List<int> controlIDs = null, GameObject targetObject3D = null)
	{
		if (nextFrame)
		{
			yield return null;
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}
		this.mainCamera = GameControl.control.mainCamera;
		this.rectTransform = base.GetComponent<RectTransform>();
		this.rootRT = this.rectTransform.root.GetComponent<RectTransform>();
		this.SetCanvasScaling();
		this.SetTipTextAndImage(locName, locDesc, tutorialImage, controlIDs);
		this.Target3D = targetObject3D;
		this.tutorialPointer2DContainer.gameObject.SetActive(true);
		if (this.currentTipIndex == this.uiTutorialTipList.Count - 1)
		{
			this.hasSeenWholeTutorial = true;
			this.closeTipButton.gameObject.SetActive(true);
			this.nextTipButton.gameObject.SetActive(false);
		}
		else
		{
			this.nextTipButton.gameObject.SetActive(true);
			this.closeTipButton.gameObject.SetActive(false);
		}
		this.previousTipButton.interactable = this.currentTipIndex != 0;
		this.centerHighlightBlocker.SetActive(!disableHighlightBlocker);
		TooltipManager.Instance.HideAll();
		if (GameControl.control.viewMgr.currentView == ViewType.PoliticalMap && GameControl.control.viewMgr != null && GameControl.control.viewMgr.earthObject != null)
		{
			GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.DeactivateRegionTooltips();
		}
		if (!targetObject.activeInHierarchy && fallbackTargetObject != null)
		{
			this.SetTipPosition(fallbackTargetObject, arrowDirOverride, false);
		}
		else
		{
			this.SetTipPosition(targetObject, arrowDirOverride, false);
		}
		UITutorialAction.Execute(tutorialAction);
		AudioManager.StopTutorialVO();
		AudioManager.PlayTutorialVO(locName);
		yield break;
	}

	// Token: 0x060016ED RID: 5869 RVA: 0x00075CF8 File Offset: 0x00073EF8
	private void SetTipTextAndImage(string locName, string locDesc, Sprite image = null, List<int> controlIDs = null)
	{
		this.tutorialTitleText.SetText(Loc.T(locName));
		string text = Loc.T(locDesc);
		if (controlIDs != null && controlIDs.Count > 0)
		{
			List<string> list = new List<string>();
			foreach (int num in controlIDs)
			{
				list.Add(TIInputManager.GetKeybindWithModifiers(num));
			}
			object[] array = list.ToArray();
			text = Loc.T(locDesc, array);
		}
		this.tutorialDescriptionText.SetText(text);
		if (image == null)
		{
			this.tutorialImage.gameObject.SetActive(false);
			return;
		}
		this.tutorialImage.sprite = image;
		this.tutorialImage.gameObject.SetActive(true);
	}

	// Token: 0x060016EE RID: 5870 RVA: 0x00075DD0 File Offset: 0x00073FD0
	public void FinishedTutorial(bool dontShowAgain = false)
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
		AudioManager.StopTutorialVO();
		base.gameObject.SetActive(false);
		if (this.parentController != null)
		{
			this.parentController.dontShowAgain = dontShowAgain;
			this.parentController.CompleteTutorial(dontShowAgain);
			UITutorialAction.Execute(this.parentController.closeTutorialAction);
		}
		if (GameControl.control.viewMgr.currentView == ViewType.PoliticalMap && GameControl.control.viewMgr != null && GameControl.control.viewMgr.earthObject != null && !GameControl.control._canvasStack.IsShowingInfoScreen())
		{
			GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ActivateRegionTooltips();
		}
	}

	// Token: 0x060016EF RID: 5871 RVA: 0x00075E9C File Offset: 0x0007409C
	private void SetTipPosition(GameObject targetObject, TutorialTip.ArrowDirection arrowDirOverride = TutorialTip.ArrowDirection.None, bool oneFrameDelay = false)
	{
		if (!GameControl.loadcycle100 || !base.gameObject.scene.isLoaded)
		{
			return;
		}
		RectTransform rectTransform = targetObject.transform as RectTransform;
		if (rectTransform != null)
		{
			this.ResetTipPosition();
			TutorialTip.ArrowDirection arrowDirection = TutorialTip.ArrowDirection.Right;
			if (arrowDirOverride != TutorialTip.ArrowDirection.None)
			{
				arrowDirection = arrowDirOverride;
			}
			bool flag = arrowDirection == TutorialTip.ArrowDirection.Left || arrowDirection == TutorialTip.ArrowDirection.Right;
			base.transform.SetParent(targetObject.transform, false);
			base.transform.localPosition = Vector3.zero;
			base.transform.SetParent(this.tipCanvasRT, true);
			base.transform.localRotation = Quaternion.identity;
			base.transform.localScale = Vector3.one;
			this.rectTransform.ForceUpdateRectTransforms();
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);
			float width = this.rootRT.rect.width;
			float num = this.rootRT.rect.height * -1f;
			float num2 = width / 2f;
			float num3 = num / 2f;
			float width2 = this.rectTransform.rect.width;
			float height = this.rectTransform.rect.height;
			float num4 = rectTransform.rect.width / 2f + (flag ? this.tipSpacing.x : this.tipSpacing.y);
			if (this.rectTransform.anchoredPosition.x > num2)
			{
				this.rectTransform.anchoredPosition -= new Vector2(num4 + width2, 0f);
				if (arrowDirOverride == TutorialTip.ArrowDirection.None)
				{
					arrowDirection = TutorialTip.ArrowDirection.Left;
				}
			}
			else
			{
				this.rectTransform.anchoredPosition += new Vector2(num4, 0f);
			}
			if (this.rectTransform.anchoredPosition.x + width2 >= width - this.tipEdgeBuffer || this.rectTransform.anchoredPosition.x <= this.tipEdgeBuffer)
			{
				this.rectTransform.anchoredPosition = new Vector2(num2 - width2 / 2f, this.rectTransform.anchoredPosition.y);
			}
			float num5 = rectTransform.rect.height / 2f + (flag ? this.tipSpacing.y : this.tipSpacing.x);
			if (this.rectTransform.anchoredPosition.y >= num3)
			{
				this.rectTransform.anchoredPosition -= new Vector2(0f, num5);
			}
			else
			{
				this.rectTransform.anchoredPosition += new Vector2(0f, num5 + height);
			}
			if (this.rectTransform.anchoredPosition.y - height <= num + this.tipEdgeBuffer || this.rectTransform.anchoredPosition.y >= -this.tipEdgeBuffer)
			{
				this.rectTransform.anchoredPosition = new Vector2(this.rectTransform.anchoredPosition.x, num3 + height / 2f);
			}
			this.Position2DPointer(targetObject, rectTransform, arrowDirection, oneFrameDelay);
		}
	}

	// Token: 0x060016F0 RID: 5872 RVA: 0x000761C0 File Offset: 0x000743C0
	private void Position2DPointer(GameObject targetObject, RectTransform targetRT, TutorialTip.ArrowDirection arrowDirection = TutorialTip.ArrowDirection.Right, bool oneFrameDelay = false)
	{
		try
		{
			if (GameControl.loadcycle100 && base.gameObject.scene.isLoaded)
			{
				if (!(base.gameObject == null) && base.gameObject.activeInHierarchy)
				{
					base.StartCoroutine(this.Position2DPointerWithDelay(targetObject, targetRT, arrowDirection, oneFrameDelay));
				}
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x060016F1 RID: 5873 RVA: 0x00076230 File Offset: 0x00074430
	private IEnumerator Position2DPointerWithDelay(GameObject targetObject, RectTransform targetRT, TutorialTip.ArrowDirection arrowDirection, bool oneFrameDelay = false)
	{
		if (this.currentRTChangeListener != null)
		{
			this.currentRTChangeListener.OnDimensionsChanged.RemoveAllListeners();
		}
		if (oneFrameDelay)
		{
			yield return null;
		}
		this.tutorialPointerContainer.sizeDelta = new Vector2(targetRT.rect.width, targetRT.rect.height);
		this.tutorialPointerContainer.position = targetObject.transform.position;
		this.SetArrowOrientation(this.tutorialPointerContainer.sizeDelta, arrowDirection);
		RectTransformChangeListener rtcl;
		if (targetObject.TryGetComponent<RectTransformChangeListener>(out rtcl))
		{
			this.currentRTChangeListener = rtcl;
			rtcl.OnDimensionsChanged.RemoveAllListeners();
			rtcl.OnDimensionsChanged.AddListener(delegate
			{
				this.SetTipPosition(rtcl.gameObject, arrowDirection, true);
			});
		}
		else
		{
			rtcl = null;
		}
		yield break;
	}

	// Token: 0x060016F2 RID: 5874 RVA: 0x0007625C File Offset: 0x0007445C
	private void ResetTipPosition()
	{
		this.tutorialPointerContainer.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		this.tutorialPointerContainer.transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	// Token: 0x060016F3 RID: 5875 RVA: 0x000762B4 File Offset: 0x000744B4
	private void SetArrowOrientation(Vector2 parentSize, TutorialTip.ArrowDirection arrowDirection = TutorialTip.ArrowDirection.Right)
	{
		switch (arrowDirection)
		{
		case TutorialTip.ArrowDirection.Left:
			this.animatedPointerContainer.localRotation = Quaternion.Euler(0f, 0f, 0f);
			this.animatedPointerRect.localPosition = new Vector2(-(parentSize.x / 2f), this.animatedPointerRect.localPosition.y);
			return;
		case TutorialTip.ArrowDirection.Top:
			this.animatedPointerContainer.localRotation = Quaternion.Euler(0f, 0f, 270f);
			this.animatedPointerRect.localPosition = new Vector2(-(parentSize.y / 2f), this.animatedPointerRect.localPosition.y);
			return;
		case TutorialTip.ArrowDirection.Bottom:
			this.animatedPointerContainer.localRotation = Quaternion.Euler(0f, 0f, 90f);
			this.animatedPointerRect.localPosition = new Vector2(-(parentSize.y / 2f), this.animatedPointerRect.localPosition.y);
			return;
		}
		this.animatedPointerContainer.localRotation = Quaternion.Euler(0f, 0f, 180f);
		this.animatedPointerRect.localPosition = new Vector2(-(parentSize.x / 2f), this.animatedPointerRect.localPosition.y);
	}

	// Token: 0x060016F4 RID: 5876 RVA: 0x00076424 File Offset: 0x00074624
	public void ClickedConfirm()
	{
		this.currentTipIndex++;
		this.previousTipButton.interactable = true;
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		if (this.parentController == null || !this.parentController.is3D)
		{
			this.SetupTip(this.uiTutorialTipList[this.currentTipIndex].tipLOCName, this.uiTutorialTipList[this.currentTipIndex].tipLOCDesc, this.uiTutorialTipList[this.currentTipIndex].targetElement, this.uiTutorialTipList[this.currentTipIndex].tipAction, this.uiTutorialTipList[this.currentTipIndex].disableHighlightBlocker, false, this.uiTutorialTipList[this.currentTipIndex].arrowDirectionOverride, this.uiTutorialTipList[this.currentTipIndex].FallbackTargetElement, this.uiTutorialTipList[this.currentTipIndex].tutorialImage, this.uiTutorialTipList[this.currentTipIndex].controlIDs, null);
			return;
		}
		this.parentController.FindAndDisplay3DTipTarget();
	}

	// Token: 0x060016F5 RID: 5877 RVA: 0x00076550 File Offset: 0x00074750
	public void ClickedBack()
	{
		if (this.currentTipIndex > 0)
		{
			this.currentTipIndex--;
		}
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
		if (this.parentController == null || !this.parentController.is3D)
		{
			this.SetupTip(this.uiTutorialTipList[this.currentTipIndex].tipLOCName, this.uiTutorialTipList[this.currentTipIndex].tipLOCDesc, this.uiTutorialTipList[this.currentTipIndex].targetElement, this.uiTutorialTipList[this.currentTipIndex].tipAction, this.uiTutorialTipList[this.currentTipIndex].disableHighlightBlocker, false, this.uiTutorialTipList[this.currentTipIndex].arrowDirectionOverride, this.uiTutorialTipList[this.currentTipIndex].FallbackTargetElement, this.uiTutorialTipList[this.currentTipIndex].tutorialImage, this.uiTutorialTipList[this.currentTipIndex].controlIDs, null);
			return;
		}
		this.parentController.FindAndDisplay3DTipTarget();
	}

	// Token: 0x060016F6 RID: 5878 RVA: 0x00076678 File Offset: 0x00074878
	public void ClickedSkipTutorial()
	{
		this.FinishedTutorial(false);
		if (this.hasSeenWholeTutorial && this.parentController != null && this.parentController.transitionToTutorial != null)
		{
			this.parentController.transitionToTutorial.ShowTutorialTips(this.parentController.transitionToMilestone, false, true);
		}
		if (this.currentRTChangeListener != null)
		{
			this.currentRTChangeListener.OnDimensionsChanged.RemoveAllListeners();
		}
	}

	// Token: 0x060016F7 RID: 5879 RVA: 0x000766F0 File Offset: 0x000748F0
	public void ClickedDontShowAgain()
	{
		this.FinishedTutorial(true);
		if (this.parentController != null && this.parentController.transitionToTutorial != null)
		{
			this.parentController.transitionToTutorial.ShowTutorialTips(this.parentController.transitionToMilestone, false, true);
		}
		if (this.currentRTChangeListener != null)
		{
			this.currentRTChangeListener.OnDimensionsChanged.RemoveAllListeners();
		}
	}

	// Token: 0x060016F8 RID: 5880 RVA: 0x00076760 File Offset: 0x00074960
	public void SetCanvasScaling()
	{
		if (this.rootCanvasScaler == null)
		{
			this.rootCanvasScaler = base.GetComponentInParent<CanvasScaler>();
		}
		GeneralControlsController generalControlsController = World.Active.GetExistingManager<CanvasManager>().StrategyHud as GeneralControlsController;
		if (generalControlsController != null)
		{
			this.rootCanvasScaler.matchWidthOrHeight = generalControlsController.GetComponent<CanvasScaler>().matchWidthOrHeight;
		}
	}

	// Token: 0x060016F9 RID: 5881 RVA: 0x000767BB File Offset: 0x000749BB
	private void OnUIScaleChanged(UIScaleSettingChange e)
	{
		this.UpdateUIScaling();
	}

	// Token: 0x060016FA RID: 5882 RVA: 0x000767C4 File Offset: 0x000749C4
	private void UpdateUIScaling()
	{
		if (this.rootCanvasScaler == null)
		{
			this.rootCanvasScaler = base.GetComponentInParent<CanvasScaler>();
		}
		this.rootCanvasScaler.referenceResolution = new Vector2(1920f, (float)TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting]);
	}

	// Token: 0x060016FB RID: 5883 RVA: 0x00076811 File Offset: 0x00074A11
	private void OnDestroy()
	{
		GameControl.eventManager.RemoveListener<UIScaleSettingChange>(new EventManager.EventDelegate<UIScaleSettingChange>(this.OnUIScaleChanged), null);
	}

	// Token: 0x04001559 RID: 5465
	public TMP_Text tutorialTitleText;

	// Token: 0x0400155A RID: 5466
	public TMP_Text tutorialDescriptionText;

	// Token: 0x0400155B RID: 5467
	public Image tutorialImage;

	// Token: 0x0400155C RID: 5468
	public RectTransform tutorialPointerContainer;

	// Token: 0x0400155D RID: 5469
	public RectTransform tutorialPointer2DContainer;

	// Token: 0x0400155E RID: 5470
	public RectTransform haloRect;

	// Token: 0x0400155F RID: 5471
	public RectTransform animatedPointerContainer;

	// Token: 0x04001560 RID: 5472
	public RectTransform animatedPointerRect;

	// Token: 0x04001561 RID: 5473
	public RectTransform arrowRect3D;

	// Token: 0x04001562 RID: 5474
	public RectTransform haloRect3D;

	// Token: 0x04001563 RID: 5475
	public GameObject centerHighlightBlocker;

	// Token: 0x04001564 RID: 5476
	public Button previousTipButton;

	// Token: 0x04001565 RID: 5477
	public Button nextTipButton;

	// Token: 0x04001566 RID: 5478
	public Button closeTipButton;

	// Token: 0x04001567 RID: 5479
	public Button hideTipButton;

	// Token: 0x04001568 RID: 5480
	public GameObject dontShowThisAgainButtonObject;

	// Token: 0x04001569 RID: 5481
	public List<UITutorial> uiTutorialTipList;

	// Token: 0x0400156A RID: 5482
	[HideInInspector]
	public UITutorialController parentController;

	// Token: 0x0400156B RID: 5483
	public int currentTipIndex;

	// Token: 0x0400156C RID: 5484
	public GameObject Target3D;

	// Token: 0x0400156D RID: 5485
	public Vector2 tipSpacing = new Vector2(75f, 25f);

	// Token: 0x0400156E RID: 5486
	public float tipEdgeBuffer = 48f;

	// Token: 0x0400156F RID: 5487
	private Camera mainCamera;

	// Token: 0x04001570 RID: 5488
	private RectTransform rectTransform;

	// Token: 0x04001571 RID: 5489
	private Canvas tipCanvas;

	// Token: 0x04001572 RID: 5490
	private RectTransform tipCanvasRT;

	// Token: 0x04001573 RID: 5491
	private RectTransform rootRT;

	// Token: 0x04001574 RID: 5492
	private CanvasScaler rootCanvasScaler;

	// Token: 0x04001575 RID: 5493
	private RectTransformChangeListener currentRTChangeListener;

	// Token: 0x04001576 RID: 5494
	private bool hasSeenWholeTutorial;

	// Token: 0x04001577 RID: 5495
	private static TutorialTip _instance;

	// Token: 0x02000C3F RID: 3135
	public enum ArrowDirection
	{
		// Token: 0x04004DC3 RID: 19907
		None,
		// Token: 0x04004DC4 RID: 19908
		Left,
		// Token: 0x04004DC5 RID: 19909
		Right,
		// Token: 0x04004DC6 RID: 19910
		Top,
		// Token: 0x04004DC7 RID: 19911
		Bottom
	}
}
