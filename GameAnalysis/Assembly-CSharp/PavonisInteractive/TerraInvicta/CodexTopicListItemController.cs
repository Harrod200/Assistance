using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008BA RID: 2234
	public class CodexTopicListItemController : MonoBehaviour
	{
		// Token: 0x06005556 RID: 21846 RVA: 0x0026CE92 File Offset: 0x0026B092
		public void Init(CodexController controller, TICodexEntryTemplate codexTemplate)
		{
			this.controller = controller;
			this.template = codexTemplate;
		}

		// Token: 0x06005557 RID: 21847 RVA: 0x0026CEA2 File Offset: 0x0026B0A2
		public void OnClickCodexTopic()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.controller.SelectTopic(this.template.dataName);
		}

		// Token: 0x06005558 RID: 21848 RVA: 0x0026CEC8 File Offset: 0x0026B0C8
		public void UpdateListItem(bool isLastEntry = false, bool nextTopicIsMainTopic = false)
		{
			bool flag = !string.IsNullOrEmpty(this.template.imgPath);
			this.topicTitle.SetText(((this.template.mainTopic || flag) ? "" : "  ") + this.template.titleText);
			this.backgroundImage.enabled = this.template.mainTopic;
			this.dividerLine.enabled = !this.template.mainTopic && !nextTopicIsMainTopic && !isLastEntry;
			this.topicIcon.gameObject.SetActive(flag);
			if (flag)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(this.template.imgPath, this.topicIcon);
			}
			this.buttonLayout.enabled = true;
			this.buttonLayout.padding.left = ((this.template.mainTopic || !flag) ? 16 : 26);
			if (this.template.unlockTech != null && this.template.unlockTech != "")
			{
				base.gameObject.SetActive(GameStateManager.GlobalResearch().finishedTechsNames.Contains(this.template.unlockTech) || GameControl.control.activePlayer.finishedProjectNames.Contains(this.template.unlockTech));
				return;
			}
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x04003B8B RID: 15243
		public CodexController controller;

		// Token: 0x04003B8C RID: 15244
		public TICodexEntryTemplate template;

		// Token: 0x04003B8D RID: 15245
		public Image backgroundImage;

		// Token: 0x04003B8E RID: 15246
		public TMP_Text topicTitle;

		// Token: 0x04003B8F RID: 15247
		public Image topicIcon;

		// Token: 0x04003B90 RID: 15248
		public Image dividerLine;

		// Token: 0x04003B91 RID: 15249
		public HorizontalLayoutGroup buttonLayout;
	}
}
