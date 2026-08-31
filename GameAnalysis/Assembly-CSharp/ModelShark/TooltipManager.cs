using System;
using System.Collections;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModelShark
{
	// Token: 0x020004B7 RID: 1207
	public class TooltipManager : MonoBehaviour
	{
		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06001B12 RID: 6930 RVA: 0x00093197 File Offset: 0x00091397
		public float tooltipDelayPrimary
		{
			get
			{
				return TIPlayerProfileManager.tooltipDelayPrimary;
			}
		}

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06001B13 RID: 6931 RVA: 0x0009319E File Offset: 0x0009139E
		public float tooltipDelaySupplemental
		{
			get
			{
				return TIPlayerProfileManager.tooltipDelaySupplemental;
			}
		}

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06001B14 RID: 6932 RVA: 0x000931A5 File Offset: 0x000913A5
		// (set) Token: 0x06001B15 RID: 6933 RVA: 0x000931AD File Offset: 0x000913AD
		public Canvas GuiCanvas { get; private set; }

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06001B16 RID: 6934 RVA: 0x000931B6 File Offset: 0x000913B6
		public string TextFieldDelimiter
		{
			get
			{
				return "%";
			}
		}

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06001B17 RID: 6935 RVA: 0x000931C0 File Offset: 0x000913C0
		public static TooltipManager Instance
		{
			get
			{
				if (TooltipManager.instance == null)
				{
					TooltipManager.instance = global::UnityEngine.Object.FindObjectOfType<TooltipManager>();
				}
				if (TooltipManager.instance == null)
				{
					return null;
				}
				if (!TooltipManager.instance.isInitialized)
				{
					TooltipManager.instance.Initialize();
				}
				return TooltipManager.instance;
			}
		}

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06001B18 RID: 6936 RVA: 0x0009320E File Offset: 0x0009140E
		// (set) Token: 0x06001B19 RID: 6937 RVA: 0x00093216 File Offset: 0x00091416
		private Canvas RootCanvas { get; set; }

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06001B1A RID: 6938 RVA: 0x0009321F File Offset: 0x0009141F
		// (set) Token: 0x06001B1B RID: 6939 RVA: 0x00093227 File Offset: 0x00091427
		public GameObject TooltipContainer { get; private set; }

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06001B1C RID: 6940 RVA: 0x00093230 File Offset: 0x00091430
		// (set) Token: 0x06001B1D RID: 6941 RVA: 0x00093238 File Offset: 0x00091438
		private GameObject TooltipContainerNoAngle { get; set; }

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06001B1E RID: 6942 RVA: 0x00093241 File Offset: 0x00091441
		// (set) Token: 0x06001B1F RID: 6943 RVA: 0x00093249 File Offset: 0x00091449
		public Dictionary<TooltipStyle, Tooltip> Tooltips { get; private set; }

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06001B20 RID: 6944 RVA: 0x00093252 File Offset: 0x00091452
		// (set) Token: 0x06001B21 RID: 6945 RVA: 0x0009325A File Offset: 0x0009145A
		public Tooltip BlockingTooltip { get; set; }

		// Token: 0x06001B22 RID: 6946 RVA: 0x00093263 File Offset: 0x00091463
		private void Awake()
		{
			TooltipManager.instance = this;
			if (!this.isInitialized)
			{
				this.Initialize();
			}
		}

		// Token: 0x06001B23 RID: 6947 RVA: 0x00093279 File Offset: 0x00091479
		private void Update()
		{
			if (!this.tooltipsEnabled && this.VisibleTooltips().Count > 0)
			{
				this.HideAll();
			}
		}

		// Token: 0x06001B24 RID: 6948 RVA: 0x00093298 File Offset: 0x00091498
		private void Initialize()
		{
			if (this.isInitialized)
			{
				return;
			}
			this.RootCanvas = CanvasHelper.GetRootCanvas();
			if (this.GuiCanvas == null)
			{
				this.GuiCanvas = this.RootCanvas;
			}
			if (this.guiCamera == null)
			{
				this.guiCamera = Camera.main;
			}
			this.Tooltips = new Dictionary<TooltipStyle, Tooltip>();
			Loc.OnLanguageChangedEvent += this.OnLanguageChangedEvent;
			this.TooltipContainer = this.CreateTooltipContainer("Tooltip Container");
			this.TooltipContainer.AddComponent<LayoutElement>();
			LayoutElement component = this.TooltipContainer.GetComponent<LayoutElement>();
			component.ignoreLayout = true;
			component.enabled = false;
			this.TooltipContainerNoAngle = this.CreateTooltipContainer("Tooltip Container (No Angle)");
			this.ResetTooltipRotation();
			this.isInitialized = true;
		}

		// Token: 0x06001B25 RID: 6949 RVA: 0x0009335C File Offset: 0x0009155C
		private GameObject CreateTooltipContainer(string containerName)
		{
			GameObject gameObject = GameObject.Find(containerName);
			if (gameObject == null)
			{
				gameObject = new GameObject(containerName);
				gameObject.transform.SetParent(this.GuiCanvas.transform, false);
				RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
				rectTransform.anchorMin = (rectTransform.offsetMin = (rectTransform.offsetMax = (rectTransform.anchoredPosition = Vector2.zero)));
				rectTransform.anchorMax = (rectTransform.localScale = Vector3.one);
				gameObject.transform.SetAsLastSibling();
			}
			return gameObject;
		}

		// Token: 0x06001B26 RID: 6950 RVA: 0x000933EC File Offset: 0x000915EC
		public void ResetTooltipRotation()
		{
			this.TooltipContainer.transform.rotation = ((this.matchRotationTo != null) ? this.matchRotationTo.transform.rotation : this.GuiCanvas.transform.rotation);
		}

		// Token: 0x06001B27 RID: 6951 RVA: 0x0009343C File Offset: 0x0009163C
		public void SetTextAndSize(TooltipTrigger trigger)
		{
			Tooltip tooltip = trigger.Tooltip;
			if (tooltip == null || trigger.parameterizedTextFields == null)
			{
				return;
			}
			if (tooltip.TextFields == null || tooltip.TextFields.Count == 0)
			{
				return;
			}
			LayoutElement mainTextContainer = tooltip.TooltipStyle.mainTextContainer;
			if (mainTextContainer == null)
			{
				if (tooltip.GameObject != null)
				{
					Debug.LogWarning(string.Format("No main text container defined on tooltip style \"{0}\". Note: This LayoutElement is needed in order to resize text appropriately.", tooltip.GameObject.name));
					return;
				}
			}
			else
			{
				mainTextContainer.preferredWidth = (float)trigger.minTextWidth;
			}
			for (int i = 0; i < tooltip.TextFields.Count; i++)
			{
				TMP_Text text = tooltip.TextFields[i].Text;
				if (text.text.Length >= 3)
				{
					for (int j = 0; j < trigger.parameterizedTextFields.Count; j++)
					{
						ParameterizedTextField parameterizedTextField = trigger.parameterizedTextFields[j];
						if (parameterizedTextField.name == text.name)
						{
							if (parameterizedTextField.valueOnDemand)
							{
								TMP_Text tmp_Text = text;
								ParameterizedTextField.BuildStringOnTooltipHover del = parameterizedTextField.del;
								tmp_Text.SetText((del != null) ? del() : null);
							}
							else if (!string.IsNullOrEmpty(parameterizedTextField.value))
							{
								text.SetText(parameterizedTextField.value);
							}
						}
						this.lastTooltip = text.text;
					}
					if (mainTextContainer != null)
					{
						if (text.preferredWidth > (float)trigger.maxTextWidth)
						{
							mainTextContainer.preferredWidth = (float)trigger.maxTextWidth;
						}
						else if (text.preferredWidth > (float)trigger.minTextWidth && text.preferredWidth > mainTextContainer.preferredWidth)
						{
							mainTextContainer.preferredWidth = text.preferredWidth;
						}
					}
				}
			}
		}

		// Token: 0x06001B28 RID: 6952 RVA: 0x000935D7 File Offset: 0x000917D7
		public IEnumerator Show(TooltipTrigger trigger)
		{
			this.attemptingToShowTooltip = true;
			if (trigger.tooltipStyle == null)
			{
				Debug.LogWarning("TooltipTrigger \"" + trigger.name + "\" has no associated TooltipStyle. Cannot show tooltip.");
				yield break;
			}
			Tooltip tooltip = trigger.Tooltip;
			Image tooltipBkgImg = tooltip.BackgroundImage;
			foreach (KeyValuePair<TooltipStyle, Tooltip> keyValuePair in this.Tooltips)
			{
				if (keyValuePair.Value.GameObject == null)
				{
					yield break;
				}
				if (keyValuePair.Value.GameObject.activeInHierarchy && keyValuePair.Value != tooltip)
				{
					TooltipTrigger tooltipTrigger = keyValuePair.Value.TooltipTrigger;
					if (tooltipTrigger != null)
					{
						tooltipTrigger.ForceHideTooltip();
					}
				}
			}
			if (tooltip.NeverRotate)
			{
				tooltip.GameObject.transform.SetParent(this.TooltipContainerNoAngle.transform, false);
			}
			if (trigger.dynamicImageFields != null)
			{
				for (int i = 0; i < trigger.dynamicImageFields.Count; i++)
				{
					for (int j = 0; j < tooltip.ImageFields.Count; j++)
					{
						if (tooltip.ImageFields[j].Name == trigger.dynamicImageFields[i].name)
						{
							if (trigger.dynamicImageFields[i].replacementSprite == null)
							{
								tooltip.ImageFields[j].Image.sprite = tooltip.ImageFields[j].Original;
							}
							else
							{
								tooltip.ImageFields[j].Image.sprite = trigger.dynamicImageFields[i].replacementSprite;
							}
						}
					}
				}
			}
			if (trigger.dynamicSectionFields != null)
			{
				for (int k = 0; k < trigger.dynamicSectionFields.Count; k++)
				{
					for (int l = 0; l < tooltip.SectionFields.Count; l++)
					{
						if (tooltip.SectionFields[l].Name == trigger.dynamicSectionFields[k].name)
						{
							tooltip.SectionFields[l].GameObject.SetActive(trigger.dynamicSectionFields[k].isOn);
						}
					}
				}
			}
			if (tooltip.TextFields != null && tooltip.TextFields.Count > 0)
			{
				float num = 14f;
				if (trigger.worldSpace)
				{
					num *= TIUtilities.UIScaleFactor();
				}
				tooltip.TextFields[0].Text.fontSize = num;
			}
			if (trigger.tipPosition != TipPosition.CanvasBottomMiddle && trigger.tipPosition != TipPosition.CanvasTopMiddle)
			{
				yield return WaitFor.Frames(2);
			}
			if (!this.attemptingToShowTooltip)
			{
				yield break;
			}
			float y = tooltip.RectTransform.sizeDelta.y;
			if (tooltip.TextFields != null && tooltip.TextFields.Count > 0)
			{
				if (y > 1200f / TIUtilities.UIScaleFactor())
				{
					tooltip.TextFields[0].Text.fontSize = 11f;
					yield return WaitFor.Frames(2);
				}
				else if (y > 1120f / TIUtilities.UIScaleFactor())
				{
					tooltip.TextFields[0].Text.fontSize = 11.5f;
					yield return WaitFor.Frames(2);
				}
				else if (y > 1040f / TIUtilities.UIScaleFactor())
				{
					tooltip.TextFields[0].Text.fontSize = 12f;
					yield return WaitFor.Frames(2);
				}
			}
			this.GuiCanvas = trigger.GetComponentInParent<Canvas>();
			if (this.GuiCanvas == null)
			{
				this.GuiCanvas = CanvasHelper.GetRootCanvas();
			}
			this.TooltipContainer.transform.SetParent(this.GuiCanvas.transform, false);
			tooltip.SetPosition(trigger, this.GuiCanvas, this.guiCamera);
			tooltip.RectTransform.localRotation = Quaternion.identity;
			tooltipBkgImg.color = trigger.backgroundTint;
			if (tooltip.IsBlocking)
			{
				this.BlockingTooltip = tooltip;
			}
			tooltip.TooltipTrigger = trigger;
			tooltip.Display(this.fadeDuration);
			tooltip.layoutGroup.enabled = true;
			this.lastTooltipTrigger = trigger;
			this.attemptingToShowTooltip = false;
			yield break;
		}

		// Token: 0x06001B29 RID: 6953 RVA: 0x000935F0 File Offset: 0x000917F0
		public void HideAll()
		{
			TooltipTrigger[] array = global::UnityEngine.Object.FindObjectsOfType<TooltipTrigger>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ForceHideTooltip();
			}
		}

		// Token: 0x06001B2A RID: 6954 RVA: 0x0009361C File Offset: 0x0009181C
		private void OnLanguageChangedEvent()
		{
			foreach (KeyValuePair<TooltipStyle, Tooltip> keyValuePair in this.Tooltips)
			{
				if (keyValuePair.Value.GameObject != null)
				{
					Loc.SwapFonts(keyValuePair.Value.GameObject);
				}
			}
		}

		// Token: 0x06001B2B RID: 6955 RVA: 0x00093690 File Offset: 0x00091890
		private void OnDestroy()
		{
			Loc.OnLanguageChangedEvent -= this.OnLanguageChangedEvent;
		}

		// Token: 0x06001B2C RID: 6956 RVA: 0x000936A4 File Offset: 0x000918A4
		public void MoveContainerToDummyCanvas(bool nextFrame = false)
		{
			if (this.GuiCanvas == null)
			{
				this.GuiCanvas = CanvasHelper.GetRootCanvas();
			}
			if (!nextFrame)
			{
				this.TooltipContainer.transform.SetParent(this.RootCanvas.transform, false);
				return;
			}
			base.StartCoroutine(this.MoveContainerToDummyNF());
		}

		// Token: 0x06001B2D RID: 6957 RVA: 0x000936F7 File Offset: 0x000918F7
		private IEnumerator MoveContainerToDummyNF()
		{
			yield return null;
			this.TooltipContainer.transform.SetParent(this.RootCanvas.transform, false);
			yield break;
		}

		// Token: 0x06001B2E RID: 6958 RVA: 0x00093708 File Offset: 0x00091908
		public List<TooltipStyle> VisibleTooltips()
		{
			List<TooltipStyle> list = new List<TooltipStyle>();
			TooltipStyle[] componentsInChildren = this.TooltipContainer.GetComponentsInChildren<TooltipStyle>(false);
			TooltipStyle[] componentsInChildren2 = this.TooltipContainerNoAngle.GetComponentsInChildren<TooltipStyle>(false);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				list.Add(componentsInChildren[i]);
			}
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				list.Add(componentsInChildren2[j]);
			}
			return list;
		}

		// Token: 0x0400171E RID: 5918
		[Tooltip("If you have multiple cameras in your scene, this is the one that will be used by ProTips.")]
		public Camera guiCamera;

		// Token: 0x0400171F RID: 5919
		[Tooltip("The RectTransform to match if you have an angled UI and you want tooltips to match the RectTransform's angle.")]
		public RectTransform matchRotationTo;

		// Token: 0x04001720 RID: 5920
		[Tooltip("Globally enable or disable tooltips.")]
		public bool tooltipsEnabled = true;

		// Token: 0x04001721 RID: 5921
		[Tooltip("When enabled, tooltips will be triggered by pressing-and-holding, not hovering over. They will be dismissed by releasing the hold, instead of hover off.")]
		public bool touchSupport;

		// Token: 0x04001722 RID: 5922
		[Tooltip("How long the tooltip fade-in transition will last. Set to 0 for increased performance.")]
		public float fadeDuration = 0.2f;

		// Token: 0x04001723 RID: 5923
		[Tooltip("Determines whether tooltips are repositioned when they would flow off the canvas. Disable for increased performance.")]
		public bool overflowProtection = true;

		// Token: 0x04001724 RID: 5924
		[Tooltip("For 3D objects, determines whether tooltips are positioned based on the object's collider bounds or mesh renderer bounds.")]
		public PositionBounds positionBounds;

		// Token: 0x04001726 RID: 5926
		public string lastTooltip;

		// Token: 0x04001727 RID: 5927
		public TooltipTrigger lastTooltipTrigger;

		// Token: 0x04001728 RID: 5928
		private static TooltipManager instance;

		// Token: 0x04001729 RID: 5929
		private bool isInitialized;

		// Token: 0x0400172F RID: 5935
		public bool attemptingToShowTooltip;
	}
}
