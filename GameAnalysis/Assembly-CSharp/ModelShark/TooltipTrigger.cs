using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ModelShark
{
	// Token: 0x020004B0 RID: 1200
	public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler
	{
		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06001AEB RID: 6891 RVA: 0x000917BE File Offset: 0x0008F9BE
		// (set) Token: 0x06001AEC RID: 6892 RVA: 0x000917C6 File Offset: 0x0008F9C6
		public Tooltip Tooltip { get; private set; }

		// Token: 0x06001AED RID: 6893 RVA: 0x000917D0 File Offset: 0x0008F9D0
		public void Start()
		{
			string name = SceneManager.GetActiveScene().name;
			if (name != null && (name == "SolarSystemScene" || name == "StartScreenScene"))
			{
				this.Initialize();
				this.Initialize();
				return;
			}
			base.enabled = false;
		}

		// Token: 0x06001AEE RID: 6894 RVA: 0x00091820 File Offset: 0x0008FA20
		private void Initialize()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (this.tooltipStyle != null)
			{
				if (!TooltipManager.Instance.Tooltips.ContainsKey(this.tooltipStyle))
				{
					TooltipStyle tooltipStyle = global::UnityEngine.Object.Instantiate<TooltipStyle>(this.tooltipStyle);
					tooltipStyle.name = this.tooltipStyle.name;
					tooltipStyle.transform.SetParent(TooltipManager.Instance.TooltipContainer.transform, false);
					Tooltip tooltip = new Tooltip
					{
						GameObject = tooltipStyle.gameObject
					};
					Loc.SwapFonts(tooltipStyle.gameObject);
					tooltip.Initialize();
					tooltip.Deactivate(false);
					tooltip.RectTransform.localScale = Vector3.one;
					TooltipManager.Instance.Tooltips.Add(this.tooltipStyle, tooltip);
				}
				this.Tooltip = TooltipManager.Instance.Tooltips[this.tooltipStyle];
				this.tooltipDelay = ((this.tooltipType == TooltipType.Primary) ? TooltipManager.Instance.tooltipDelayPrimary : TooltipManager.Instance.tooltipDelaySupplemental);
			}
			else
			{
				Debug.LogError("Missing tooltipStyle in inspector for " + base.transform.name);
			}
			this.isInitialized = true;
		}

		// Token: 0x06001AEF RID: 6895 RVA: 0x0009194C File Offset: 0x0008FB4C
		private void Update()
		{
			if (this.hoverTimer > 0f)
			{
				this.hoverTimer += Time.unscaledDeltaTime;
			}
			if (this.hoverTimer > this.tooltipDelay)
			{
				this.hoverTimer = 0f;
				this.StartHover();
			}
			if (this.popupTimer > 0f)
			{
				this.popupTimer += Time.unscaledDeltaTime;
				if (this.popupTimer > this.popupTime && this.Tooltip != null && !this.Tooltip.StaysOpen)
				{
					this.popupTimer = 0f;
					this.Tooltip.Deactivate(false);
				}
			}
		}

		// Token: 0x06001AF0 RID: 6896 RVA: 0x000919F0 File Offset: 0x0008FBF0
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (TooltipManager.Instance.touchSupport)
			{
				return;
			}
			if (this.isRemotelyActivated || TooltipManager.Instance.BlockingTooltip != null)
			{
				return;
			}
			if (this.Tooltip == null)
			{
				Debug.LogError("Missing Tooltip for " + base.transform.name + ", check tooltipstyle in the inspector");
				if (!this.isInitialized)
				{
					Debug.LogError("No Tooltip due to no Initialization");
				}
				return;
			}
			if (this.Tooltip.GameObject == null)
			{
				Log.Warn("Missing GameObject for Tooltip", Array.Empty<object>());
				this.CreateTooltipRuntime(this.tooltipStyle);
				this.Tooltip.Deactivate(false);
				return;
			}
			if (this.Tooltip.GameObject.activeInHierarchy && this.Tooltip.TooltipTrigger == this)
			{
				return;
			}
			this.tooltipDelay = ((this.tooltipType == TooltipType.Primary) ? TooltipManager.Instance.tooltipDelayPrimary : TooltipManager.Instance.tooltipDelaySupplemental);
			this.hoverTimer = 0.001f;
		}

		// Token: 0x06001AF1 RID: 6897 RVA: 0x00091AEC File Offset: 0x0008FCEC
		public void OnMouseOver()
		{
			if (EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}
			if (this.isMouseOver)
			{
				return;
			}
			if (TooltipManager.Instance.touchSupport)
			{
				return;
			}
			if (this.isRemotelyActivated || TooltipManager.Instance.BlockingTooltip != null)
			{
				return;
			}
			if (this.Tooltip.GameObject == null)
			{
				Log.Warn("Missing GameObject for Tooltip", Array.Empty<object>());
				this.CreateTooltipRuntime(this.tooltipStyle);
				this.Tooltip.Deactivate(false);
				return;
			}
			if (this.Tooltip.GameObject.activeInHierarchy && this.Tooltip.TooltipTrigger == this)
			{
				return;
			}
			this.hoverTimer = 0.001f;
			this.isMouseOver = true;
		}

		// Token: 0x06001AF2 RID: 6898 RVA: 0x00091BA4 File Offset: 0x0008FDA4
		private void CreateTooltipRuntime(TooltipStyle tooltipStyle)
		{
			if (TooltipManager.Instance.Tooltips.ContainsKey(tooltipStyle))
			{
				TooltipManager.Instance.Tooltips.Remove(tooltipStyle);
			}
			if (!TooltipManager.Instance.Tooltips.ContainsKey(tooltipStyle))
			{
				TooltipStyle tooltipStyle2 = global::UnityEngine.Object.Instantiate<TooltipStyle>(tooltipStyle);
				tooltipStyle2.name = tooltipStyle.name;
				tooltipStyle2.transform.SetParent(TooltipManager.Instance.TooltipContainer.transform, false);
				Tooltip tooltip = new Tooltip
				{
					GameObject = tooltipStyle2.gameObject
				};
				Loc.SwapFonts(tooltipStyle2.gameObject);
				tooltip.Initialize();
				tooltip.Deactivate(false);
				tooltip.RectTransform.localScale = Vector3.one;
				TooltipManager.Instance.Tooltips.Add(tooltipStyle, tooltip);
				this.Tooltip = TooltipManager.Instance.Tooltips[tooltipStyle];
				Debug.LogWarning("Recreating Destroyed Tooltip");
			}
		}

		// Token: 0x06001AF3 RID: 6899 RVA: 0x00091C84 File Offset: 0x0008FE84
		public void OnMouseDown()
		{
			if (this.isMouseDown)
			{
				return;
			}
			if (!TooltipManager.Instance.touchSupport)
			{
				return;
			}
			if (this.isRemotelyActivated || TooltipManager.Instance.BlockingTooltip != null)
			{
				return;
			}
			if (this.Tooltip.GameObject.activeInHierarchy && this.Tooltip.TooltipTrigger == this)
			{
				return;
			}
			this.hoverTimer = 0.001f;
			this.isMouseDown = true;
		}

		// Token: 0x06001AF4 RID: 6900 RVA: 0x00091CF4 File Offset: 0x0008FEF4
		public void OnMouseExit()
		{
			if (EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}
			if (TooltipManager.Instance.touchSupport)
			{
				return;
			}
			this.isMouseOver = false;
			this.StopHover(false);
		}

		// Token: 0x06001AF5 RID: 6901 RVA: 0x00091D20 File Offset: 0x0008FF20
		public void OnPointerDown(PointerEventData eventData)
		{
			if (!TooltipManager.Instance.touchSupport)
			{
				return;
			}
			if (this.isRemotelyActivated || TooltipManager.Instance.BlockingTooltip != null)
			{
				return;
			}
			if (this.Tooltip.GameObject.activeInHierarchy && this.Tooltip.TooltipTrigger == this)
			{
				return;
			}
			this.hoverTimer = 0.001f;
		}

		// Token: 0x06001AF6 RID: 6902 RVA: 0x00091D80 File Offset: 0x0008FF80
		public void OnSelect(BaseEventData eventData)
		{
			if (TooltipManager.Instance.touchSupport)
			{
				return;
			}
			if (this.isRemotelyActivated || TooltipManager.Instance.BlockingTooltip != null)
			{
				return;
			}
			if (this.Tooltip.GameObject.activeInHierarchy && this.Tooltip.TooltipTrigger == this)
			{
				return;
			}
			this.hoverTimer = 0.001f;
		}

		// Token: 0x06001AF7 RID: 6903 RVA: 0x00091DE0 File Offset: 0x0008FFE0
		public void OnPointerExit(PointerEventData eventData)
		{
			if (TooltipManager.Instance != null && TooltipManager.Instance.touchSupport)
			{
				return;
			}
			this.StopHover(false);
			if (this.disablePendingTooltipsOnMouseExit)
			{
				this.DisablePendingTooltips();
			}
		}

		// Token: 0x06001AF8 RID: 6904 RVA: 0x00091E11 File Offset: 0x00090011
		public void OnPointerUp(PointerEventData eventData)
		{
			if (!TooltipManager.Instance.touchSupport)
			{
				return;
			}
			this.StopHover(false);
		}

		// Token: 0x06001AF9 RID: 6905 RVA: 0x00091E27 File Offset: 0x00090027
		public void OnMouseUp()
		{
			if (!TooltipManager.Instance.touchSupport)
			{
				return;
			}
			this.isMouseDown = false;
			this.StopHover(false);
		}

		// Token: 0x06001AFA RID: 6906 RVA: 0x00091E44 File Offset: 0x00090044
		public void OnDeselect(BaseEventData eventData)
		{
			if (TooltipManager.Instance != null && TooltipManager.Instance.touchSupport)
			{
				return;
			}
			this.StopHover(false);
		}

		// Token: 0x06001AFB RID: 6907 RVA: 0x00091E68 File Offset: 0x00090068
		public void StartHover()
		{
			if (!TooltipManager.Instance.tooltipsEnabled)
			{
				return;
			}
			if (this.minTextWidth > this.maxTextWidth)
			{
				this.maxTextWidth = this.minTextWidth;
			}
			this.Tooltip.WarmUp();
			this.Tooltip.ResetParameterizedFields();
			this.Tooltip.StaysOpen = this.staysOpen;
			this.Tooltip.NeverRotate = this.neverRotate;
			this.Tooltip.IsBlocking = this.isBlocking;
			TooltipManager.Instance.SetTextAndSize(this);
			base.StartCoroutine(TooltipManager.Instance.Show(this));
		}

		// Token: 0x06001AFC RID: 6908 RVA: 0x00091F04 File Offset: 0x00090104
		public void ForceRefreshTooltip()
		{
			if (this.Tooltip != null && this.Tooltip.GameObject != null && base.enabled && base.gameObject.activeInHierarchy)
			{
				this.Tooltip.Deactivate(false);
				this.Tooltip.WarmUp();
				this.Tooltip.ResetParameterizedFields();
				this.Tooltip.StaysOpen = this.staysOpen;
				this.Tooltip.NeverRotate = this.neverRotate;
				this.Tooltip.IsBlocking = this.isBlocking;
				TooltipManager.Instance.SetTextAndSize(this);
				base.StartCoroutine(TooltipManager.Instance.Show(this));
			}
		}

		// Token: 0x06001AFD RID: 6909 RVA: 0x00091FBC File Offset: 0x000901BC
		public void ForceRefreshTooltipIfOpen()
		{
			if (this.Tooltip != null && this.Tooltip.GameObject != null && base.enabled && base.gameObject.activeInHierarchy && TooltipManager.Instance.lastTooltipTrigger == this)
			{
				TooltipManager.Instance.SetTextAndSize(this);
			}
		}

		// Token: 0x06001AFE RID: 6910 RVA: 0x00092018 File Offset: 0x00090218
		public void ForceHideTooltip()
		{
			this.hoverTimer = (this.popupTimer = 0f);
			if (this.Tooltip != null)
			{
				this.Tooltip.Deactivate(false);
			}
		}

		// Token: 0x06001AFF RID: 6911 RVA: 0x0009204D File Offset: 0x0009024D
		public void DisablePendingTooltips()
		{
			TooltipManager.Instance.attemptingToShowTooltip = false;
		}

		// Token: 0x06001B00 RID: 6912 RVA: 0x0009205C File Offset: 0x0009025C
		public void StopHover(bool delayReparentOneFrame = false)
		{
			if (this.Tooltip == null || this.Tooltip.GameObject == null)
			{
				return;
			}
			if (this.isRemotelyActivated || (this.Tooltip.StaysOpen && this.Tooltip.IsBlocking))
			{
				return;
			}
			if (this.Tooltip.StaysOpen && this.Tooltip.TooltipTrigger == this)
			{
				return;
			}
			this.hoverTimer = 0f;
			if (this.Tooltip != null)
			{
				this.Tooltip.Deactivate(delayReparentOneFrame);
			}
		}

		// Token: 0x06001B01 RID: 6913 RVA: 0x000920E8 File Offset: 0x000902E8
		public void Popup(float duration, GameObject triggeredBy)
		{
			if (!TooltipManager.Instance.tooltipsEnabled)
			{
				return;
			}
			if (this.popupTimer > 0f || TooltipManager.Instance.BlockingTooltip != null)
			{
				return;
			}
			this.Initialize();
			if (this.minTextWidth > this.maxTextWidth)
			{
				this.maxTextWidth = this.minTextWidth;
			}
			this.Tooltip.WarmUp();
			this.Tooltip.StaysOpen = this.staysOpen;
			this.Tooltip.NeverRotate = this.neverRotate;
			this.Tooltip.IsBlocking = this.isBlocking;
			TooltipManager.Instance.SetTextAndSize(this);
			base.StartCoroutine(TooltipManager.Instance.Show(this));
			this.popupTimer = 0.001f;
			this.popupTime = duration;
		}

		// Token: 0x06001B02 RID: 6914 RVA: 0x000921AC File Offset: 0x000903AC
		public void SetDelegate(string parameterName, ParameterizedTextField.BuildStringOnTooltipHover del)
		{
			this.SetText(parameterName, new StringBuilder(base.name).Append(":").Append(parameterName).Append(" delegate")
				.ToString());
			foreach (ParameterizedTextField parameterizedTextField in this.parameterizedTextFields)
			{
				if (!(parameterizedTextField.name != parameterName))
				{
					parameterizedTextField.del = del;
					parameterizedTextField.valueOnDemand = true;
				}
			}
		}

		// Token: 0x06001B03 RID: 6915 RVA: 0x00092248 File Offset: 0x00090448
		public void SetText(string parameterName, string text)
		{
			if (this.parameterizedTextFields == null)
			{
				this.parameterizedTextFields = new List<ParameterizedTextField>();
			}
			for (int i = 0; i < this.parameterizedTextFields.Count; i++)
			{
				ParameterizedTextField parameterizedTextField = this.parameterizedTextFields[i];
				if (parameterizedTextField.name == parameterName)
				{
					parameterizedTextField.value = text;
					return;
				}
			}
			this.parameterizedTextFields.Add(new ParameterizedTextField
			{
				name = parameterName,
				placeholder = string.Format("{0}{1}{0}", TooltipManager.Instance.TextFieldDelimiter, parameterName),
				value = text
			});
		}

		// Token: 0x06001B04 RID: 6916 RVA: 0x000922DC File Offset: 0x000904DC
		public void SetImage(string parameterName, Sprite sprite)
		{
			if (this.dynamicImageFields == null)
			{
				this.dynamicImageFields = new List<DynamicImageField>();
			}
			bool flag = false;
			foreach (DynamicImageField dynamicImageField in this.dynamicImageFields)
			{
				if (!(dynamicImageField.name != parameterName))
				{
					dynamicImageField.replacementSprite = sprite;
					flag = true;
				}
			}
			if (!flag)
			{
				this.dynamicImageFields.Add(new DynamicImageField
				{
					name = parameterName,
					placeholderSprite = null,
					replacementSprite = sprite
				});
			}
		}

		// Token: 0x06001B05 RID: 6917 RVA: 0x0009237C File Offset: 0x0009057C
		public void TurnSectionOn(string parameterName)
		{
			this.ToggleSection(parameterName, true);
		}

		// Token: 0x06001B06 RID: 6918 RVA: 0x00092386 File Offset: 0x00090586
		public void TurnSectionOff(string parameterName)
		{
			this.ToggleSection(parameterName, false);
		}

		// Token: 0x06001B07 RID: 6919 RVA: 0x00092390 File Offset: 0x00090590
		private void ToggleSection(string parameterName, bool isOn)
		{
			if (this.dynamicSectionFields == null)
			{
				this.dynamicSectionFields = new List<DynamicSectionField>();
			}
			bool flag = false;
			foreach (DynamicSectionField dynamicSectionField in this.dynamicSectionFields)
			{
				if (!(dynamicSectionField.name != parameterName))
				{
					dynamicSectionField.isOn = isOn;
					flag = true;
				}
			}
			if (!flag)
			{
				this.dynamicSectionFields.Add(new DynamicSectionField
				{
					name = parameterName,
					isOn = isOn
				});
			}
		}

		// Token: 0x06001B08 RID: 6920 RVA: 0x0009242C File Offset: 0x0009062C
		public void OnDisable()
		{
			if (!base.gameObject.scene.isLoaded || !GameControl.loadcycle100)
			{
				return;
			}
			if (TooltipManager.Instance != null)
			{
				if (TooltipManager.Instance.touchSupport)
				{
					return;
				}
				if (TooltipManager.Instance.lastTooltipTrigger != null && TooltipManager.Instance.lastTooltipTrigger == this)
				{
					this.StopHover(true);
				}
			}
		}

		// Token: 0x040016F0 RID: 5872
		[HideInInspector]
		public TooltipStyle tooltipStyle;

		// Token: 0x040016F1 RID: 5873
		[HideInInspector]
		public List<ParameterizedTextField> parameterizedTextFields;

		// Token: 0x040016F2 RID: 5874
		[HideInInspector]
		public List<DynamicImageField> dynamicImageFields;

		// Token: 0x040016F3 RID: 5875
		[HideInInspector]
		public List<DynamicSectionField> dynamicSectionFields;

		// Token: 0x040016F4 RID: 5876
		[HideInInspector]
		public bool isRemotelyActivated;

		// Token: 0x040016F6 RID: 5878
		public bool worldSpace;

		// Token: 0x040016F7 RID: 5879
		public TooltipType tooltipType = TooltipType.Supplemental;

		// Token: 0x040016F8 RID: 5880
		[Tooltip("Controls the color and fade amount of the tooltip background.")]
		public Color backgroundTint = Color.white;

		// Token: 0x040016F9 RID: 5881
		public TipPosition tipPosition;

		// Token: 0x040016FA RID: 5882
		public int minTextWidth;

		// Token: 0x040016FB RID: 5883
		public int maxTextWidth = 300;

		// Token: 0x040016FC RID: 5884
		[Tooltip("JC - prevents essential tooltip elements from being destroyed when dropdowns are closed and destroyed by TMP")]
		public bool disablePendingTooltipsOnMouseExit;

		// Token: 0x040016FD RID: 5885
		[HideInInspector]
		[Tooltip("Once open, this tooltip will stay open until the user hovers over another tooltip trigger or something (like a script) manually closes it.")]
		public bool staysOpen;

		// Token: 0x040016FE RID: 5886
		[HideInInspector]
		[Tooltip("If true, this tooltip will not be angled/rotated along with other tooltips (see MatchRotationTo on TooltipManager).")]
		public bool neverRotate;

		// Token: 0x040016FF RID: 5887
		[HideInInspector]
		[Tooltip("While open, this tooltip will prevent any other tooltips from triggering. Something (like a script) will need to manually close it.")]
		public bool isBlocking;

		// Token: 0x04001700 RID: 5888
		private float hoverTimer;

		// Token: 0x04001701 RID: 5889
		private float popupTimer;

		// Token: 0x04001702 RID: 5890
		private float tooltipDelay = 0.2f;

		// Token: 0x04001703 RID: 5891
		private float popupTime = 2f;

		// Token: 0x04001704 RID: 5892
		private bool isInitialized;

		// Token: 0x04001705 RID: 5893
		private bool isMouseOver;

		// Token: 0x04001706 RID: 5894
		private bool isMouseDown;
	}
}
