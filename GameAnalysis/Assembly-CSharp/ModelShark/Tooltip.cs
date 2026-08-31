using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModelShark
{
	// Token: 0x020004BC RID: 1212
	public class Tooltip
	{
		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06001B33 RID: 6963 RVA: 0x000937A2 File Offset: 0x000919A2
		// (set) Token: 0x06001B34 RID: 6964 RVA: 0x000937AA File Offset: 0x000919AA
		public RectTransform RectTransform { get; set; }

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06001B35 RID: 6965 RVA: 0x000937B3 File Offset: 0x000919B3
		// (set) Token: 0x06001B36 RID: 6966 RVA: 0x000937BB File Offset: 0x000919BB
		public TooltipStyle TooltipStyle { get; set; }

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x06001B37 RID: 6967 RVA: 0x000937C4 File Offset: 0x000919C4
		// (set) Token: 0x06001B38 RID: 6968 RVA: 0x000937CC File Offset: 0x000919CC
		public GameObject GameObject { get; set; }

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x06001B39 RID: 6969 RVA: 0x000937D5 File Offset: 0x000919D5
		// (set) Token: 0x06001B3A RID: 6970 RVA: 0x000937DD File Offset: 0x000919DD
		public List<TextField> TextFields { get; set; }

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06001B3B RID: 6971 RVA: 0x000937E6 File Offset: 0x000919E6
		// (set) Token: 0x06001B3C RID: 6972 RVA: 0x000937EE File Offset: 0x000919EE
		public List<ImageField> ImageFields { get; set; }

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06001B3D RID: 6973 RVA: 0x000937F7 File Offset: 0x000919F7
		// (set) Token: 0x06001B3E RID: 6974 RVA: 0x000937FF File Offset: 0x000919FF
		public List<SectionField> SectionFields { get; set; }

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06001B3F RID: 6975 RVA: 0x00093808 File Offset: 0x00091A08
		// (set) Token: 0x06001B40 RID: 6976 RVA: 0x00093810 File Offset: 0x00091A10
		public Image BackgroundImage { get; set; }

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x06001B41 RID: 6977 RVA: 0x00093819 File Offset: 0x00091A19
		// (set) Token: 0x06001B42 RID: 6978 RVA: 0x00093821 File Offset: 0x00091A21
		public CanvasRenderer[] CanvasRenderers { get; set; }

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06001B43 RID: 6979 RVA: 0x0009382A File Offset: 0x00091A2A
		// (set) Token: 0x06001B44 RID: 6980 RVA: 0x00093832 File Offset: 0x00091A32
		public Graphic[] Graphics { get; set; }

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06001B45 RID: 6981 RVA: 0x0009383B File Offset: 0x00091A3B
		// (set) Token: 0x06001B46 RID: 6982 RVA: 0x00093843 File Offset: 0x00091A43
		public LayoutGroup[] LayoutGroups { get; set; }

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06001B47 RID: 6983 RVA: 0x0009384C File Offset: 0x00091A4C
		// (set) Token: 0x06001B48 RID: 6984 RVA: 0x00093854 File Offset: 0x00091A54
		public bool StaysOpen { get; set; }

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06001B49 RID: 6985 RVA: 0x0009385D File Offset: 0x00091A5D
		// (set) Token: 0x06001B4A RID: 6986 RVA: 0x00093865 File Offset: 0x00091A65
		public bool NeverRotate { get; set; }

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06001B4B RID: 6987 RVA: 0x0009386E File Offset: 0x00091A6E
		// (set) Token: 0x06001B4C RID: 6988 RVA: 0x00093876 File Offset: 0x00091A76
		public bool IsBlocking { get; set; }

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06001B4D RID: 6989 RVA: 0x0009387F File Offset: 0x00091A7F
		// (set) Token: 0x06001B4E RID: 6990 RVA: 0x00093886 File Offset: 0x00091A86
		public static string Delimiter { get; set; }

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06001B4F RID: 6991 RVA: 0x0009388E File Offset: 0x00091A8E
		// (set) Token: 0x06001B50 RID: 6992 RVA: 0x00093896 File Offset: 0x00091A96
		public TooltipTrigger TooltipTrigger { get; set; }

		// Token: 0x06001B51 RID: 6993 RVA: 0x000938A0 File Offset: 0x00091AA0
		public void Initialize()
		{
			if (string.IsNullOrEmpty(Tooltip.Delimiter))
			{
				Tooltip.Delimiter = TooltipManager.Instance.TextFieldDelimiter;
			}
			this.RectTransform = this.GameObject.GetComponent<RectTransform>();
			this.TooltipStyle = this.GameObject.GetComponent<TooltipStyle>();
			this.BackgroundImage = this.GameObject.GetComponent<Image>();
			this.CanvasRenderers = this.GameObject.GetComponentsInChildren<CanvasRenderer>(true);
			this.Graphics = this.GameObject.GetComponentsInChildren<Graphic>(true);
			this.LayoutGroups = this.GameObject.GetComponentsInChildren<LayoutGroup>(true);
			TMP_Text[] componentsInChildren = this.GameObject.GetComponentsInChildren<TMP_Text>(true);
			this.TextFields = new List<TextField>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].text.Contains(Tooltip.Delimiter))
				{
					this.TextFields.Add(new TextField
					{
						Text = componentsInChildren[i],
						Original = componentsInChildren[i].text
					});
				}
				if (componentsInChildren[i].rectTransform != null && componentsInChildren[i].rectTransform.sizeDelta == Vector2.zero)
				{
					componentsInChildren[i].rectTransform.sizeDelta = new Vector2(150f, 24f);
				}
			}
			List<DynamicImage> list = this.GameObject.GetComponentsInChildren<DynamicImage>(true).ToList<DynamicImage>();
			this.ImageFields = new List<ImageField>();
			for (int j = 0; j < list.Count; j++)
			{
				Image component = list[j].GetComponent<Image>();
				this.ImageFields.Add(new ImageField
				{
					Image = component,
					Name = list[j].placeholderName.Trim(Tooltip.Delimiter.ToCharArray()),
					Original = component.sprite
				});
			}
			List<DynamicSection> list2 = this.GameObject.GetComponentsInChildren<DynamicSection>(true).ToList<DynamicSection>();
			this.SectionFields = new List<SectionField>();
			for (int k = 0; k < list2.Count; k++)
			{
				GameObject gameObject = list2[k].gameObject;
				this.SectionFields.Add(new SectionField
				{
					GameObject = gameObject,
					Name = list2[k].placeholderName.Trim(Tooltip.Delimiter.ToCharArray()),
					Original = gameObject.activeSelf
				});
			}
			this.layoutGroup = this.GameObject.GetComponent<HorizontalOrVerticalLayoutGroup>();
		}

		// Token: 0x06001B52 RID: 6994 RVA: 0x00093B00 File Offset: 0x00091D00
		public void WarmUp()
		{
			if (this.GameObject != null)
			{
				this.GameObject.SetActive(true);
				for (int i = 0; i < this.CanvasRenderers.Length; i++)
				{
					this.CanvasRenderers[i].SetAlpha(0f);
				}
			}
			for (int j = 0; j < this.LayoutGroups.Length; j++)
			{
				this.LayoutGroups[j].enabled = true;
			}
		}

		// Token: 0x06001B53 RID: 6995 RVA: 0x00093B70 File Offset: 0x00091D70
		public void Deactivate(bool delayReparentOneFrame = false)
		{
			if (TooltipManager.Instance == null)
			{
				return;
			}
			if (this.GameObject == null)
			{
				return;
			}
			if (this.RectTransform == null)
			{
				return;
			}
			this.ResetParameterizedFields();
			if (TooltipManager.Instance.BlockingTooltip == this)
			{
				TooltipManager.Instance.BlockingTooltip = null;
			}
			this.GameObject.SetActive(false);
			this.RectTransform.SetParent(TooltipManager.Instance.TooltipContainer.transform, false);
			TooltipManager.Instance.MoveContainerToDummyCanvas(delayReparentOneFrame);
		}

		// Token: 0x06001B54 RID: 6996 RVA: 0x00093BFC File Offset: 0x00091DFC
		public void ResetParameterizedFields()
		{
			if (TooltipManager.Instance == null)
			{
				return;
			}
			if (this.GameObject == null)
			{
				return;
			}
			if (this.RectTransform == null)
			{
				return;
			}
			for (int i = 0; i < this.TextFields.Count; i++)
			{
				this.TextFields[i].Text.text = this.TextFields[i].Original;
			}
			for (int j = 0; j < this.ImageFields.Count; j++)
			{
				this.ImageFields[j].Image.sprite = this.ImageFields[j].Original;
			}
		}

		// Token: 0x06001B55 RID: 6997 RVA: 0x00093CB0 File Offset: 0x00091EB0
		public void Display(float fadeDuration)
		{
			if (fadeDuration > 0f)
			{
				for (int i = 0; i < this.Graphics.Length; i++)
				{
					this.Graphics[i].CrossFadeAlpha(1f, fadeDuration, true);
				}
				return;
			}
			for (int j = 0; j < this.CanvasRenderers.Length; j++)
			{
				this.CanvasRenderers[j].SetAlpha(1f);
			}
		}

		// Token: 0x0400174C RID: 5964
		public HorizontalOrVerticalLayoutGroup layoutGroup;
	}
}
