using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008C8 RID: 2248
	public class TechSearchListItemController : MonoBehaviour
	{
		// Token: 0x0600564C RID: 22092 RVA: 0x00277EA4 File Offset: 0x002760A4
		public void UpdateItem(ResearchScreenController controller, ChildTechGridItemController techTreeItem)
		{
			this.controller = controller;
			this.techInTree = techTreeItem;
			this.techIcon.sprite = this.techInTree.techIcon.sprite;
			this.techName.SetText(this.techInTree.techName.text);
			base.gameObject.SetActive(true);
			techTreeItem.UpdateTooltip();
			this.techTooltip.SetText("BodyText", techTreeItem.toolTipString);
		}

		// Token: 0x0600564D RID: 22093 RVA: 0x00277F1D File Offset: 0x0027611D
		public void Hide()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600564E RID: 22094 RVA: 0x00277F2B File Offset: 0x0027612B
		public void OnClickSearchResult(int treeType)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.controller.GotoSearchItem(this.techInTree, (ResearchScreenController.techTreeType)treeType);
		}

		// Token: 0x04003D54 RID: 15700
		private ResearchScreenController controller;

		// Token: 0x04003D55 RID: 15701
		private ChildTechGridItemController techInTree;

		// Token: 0x04003D56 RID: 15702
		public Image techIcon;

		// Token: 0x04003D57 RID: 15703
		public TMP_Text techName;

		// Token: 0x04003D58 RID: 15704
		public TooltipTrigger techTooltip;
	}
}
