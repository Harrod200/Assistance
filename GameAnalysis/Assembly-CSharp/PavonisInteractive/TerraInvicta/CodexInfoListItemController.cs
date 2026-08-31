using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008B9 RID: 2233
	public class CodexInfoListItemController : MonoBehaviour
	{
		// Token: 0x06005550 RID: 21840 RVA: 0x0026CA8F File Offset: 0x0026AC8F
		public void Init(CodexController controller, TICodexEntryTemplate codexTemplate, int locIndex)
		{
			this.usingTemplate = false;
			this.controller = controller;
			this.template = codexTemplate;
			this.locTextIndex = locIndex;
			this.templateImagePath = string.Empty;
		}

		// Token: 0x06005551 RID: 21841 RVA: 0x0026CAB8 File Offset: 0x0026ACB8
		public void InitCodexTemplateItemWithIcon(CodexController controller, TICodexEntryTemplate codexTemplate, string templateText, string iconPath)
		{
			this.usingTemplate = true;
			this.controller = controller;
			this.template = codexTemplate;
			this.templateOverrideString = templateText;
			this.templateImagePath = iconPath;
			this.titleContainer.SetActive(false);
			this.infoIllustration.gameObject.SetActive(false);
		}

		// Token: 0x06005552 RID: 21842 RVA: 0x0026CB08 File Offset: 0x0026AD08
		public void InitCodexTemplateItem(CodexController controller, TICodexEntryTemplate codexTemplate, string templateText)
		{
			this.usingTemplate = true;
			this.controller = controller;
			this.template = codexTemplate;
			this.templateOverrideString = templateText;
			this.templateImagePath = "";
			this.titleContainer.SetActive(false);
			this.infoIllustration.gameObject.SetActive(false);
		}

		// Token: 0x06005553 RID: 21843 RVA: 0x0026CB5C File Offset: 0x0026AD5C
		public void UpdateListItem()
		{
			if (!this.usingTemplate)
			{
				if (this.locTextIndex == 0)
				{
					this.titleContainer.SetActive(true);
					this.title.SetText(this.template.titleText);
					TICodexEntryTemplate ticodexEntryTemplate = this.controller.allCodexEntries.FirstOrDefault<TICodexEntryTemplate>((TICodexEntryTemplate x) => x.index == Mathf.Floor(this.template.index));
					if (ticodexEntryTemplate != null)
					{
						this.subTitle.SetText(ticodexEntryTemplate.titleText);
					}
					this.subTitle.gameObject.SetActive(!this.template.mainTopic && ticodexEntryTemplate != null);
					if (!string.IsNullOrEmpty(this.template.illustrationPath))
					{
						this.infoIllustration.gameObject.SetActive(true);
						GameControl.assetLoader.LoadAssetForImageAssignment(this.template.illustrationPath, this.infoIllustration);
					}
					else
					{
						this.infoIllustration.gameObject.SetActive(false);
					}
				}
				else
				{
					this.infoIllustration.gameObject.SetActive(false);
					this.titleContainer.SetActive(false);
				}
				string text = new StringBuilder(this.template.locPath).Append(this.locTextIndex.ToString()).ToString();
				string text2 = Loc.T(text);
				if (text2.Contains("<LOC,"))
				{
					text = text2.Split(this.locTagSeparator, StringSplitOptions.None)[1].Split(this.locTagEndSeparator, StringSplitOptions.None)[0];
				}
				List<string> list = new List<string>();
				int num = 0;
				using (List<string>.Enumerator enumerator = this.template.textParams.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!string.IsNullOrEmpty(enumerator.Current))
						{
							list.Add(this.template.textParamToStrValue(num++).ToString());
						}
					}
				}
				string text3 = text;
				object[] array = list.ToArray();
				string text4 = Loc.T(text3, array);
				this.infoText.SetText(text4);
				if (!string.IsNullOrEmpty(this.template.imgPath) && this.locTextIndex == 0)
				{
					this.titleImage.gameObject.SetActive(true);
					this.infoImage.gameObject.SetActive(false);
					GameControl.assetLoader.LoadAssetForImageAssignment(this.template.imgPath, this.titleImage);
					return;
				}
				this.titleImage.gameObject.SetActive(false);
				this.infoImage.gameObject.SetActive(false);
				this.infoImage.sprite = null;
				return;
			}
			else
			{
				this.infoText.SetText(this.templateOverrideString);
				if (this.templateImagePath != null && this.templateImagePath != "")
				{
					this.infoImage.gameObject.SetActive(true);
					GameControl.assetLoader.LoadAssetForImageAssignment(this.templateImagePath, this.infoImage);
					return;
				}
				this.infoImage.gameObject.SetActive(false);
				this.infoImage.sprite = null;
				return;
			}
		}

		// Token: 0x04003B7C RID: 15228
		public CodexController controller;

		// Token: 0x04003B7D RID: 15229
		public TICodexEntryTemplate template;

		// Token: 0x04003B7E RID: 15230
		public int locTextIndex;

		// Token: 0x04003B7F RID: 15231
		public TMP_Text infoText;

		// Token: 0x04003B80 RID: 15232
		public Image infoImage;

		// Token: 0x04003B81 RID: 15233
		public Image titleImage;

		// Token: 0x04003B82 RID: 15234
		public Image infoIllustration;

		// Token: 0x04003B83 RID: 15235
		public TMP_Text title;

		// Token: 0x04003B84 RID: 15236
		public TMP_Text subTitle;

		// Token: 0x04003B85 RID: 15237
		public GameObject titleContainer;

		// Token: 0x04003B86 RID: 15238
		public bool usingTemplate;

		// Token: 0x04003B87 RID: 15239
		public string templateOverrideString;

		// Token: 0x04003B88 RID: 15240
		public string templateImagePath;

		// Token: 0x04003B89 RID: 15241
		private readonly string[] locTagSeparator = new string[] { "<LOC," };

		// Token: 0x04003B8A RID: 15242
		private readonly string[] locTagEndSeparator = new string[] { ">" };
	}
}
