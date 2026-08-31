using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000833 RID: 2099
	public class CouncilorAugmentationListItemController : MonoBehaviour
	{
		// Token: 0x06004C0D RID: 19469 RVA: 0x001FF679 File Offset: 0x001FD879
		public void Init(CouncilGridController controller, TICouncilorState councilor, CouncilorAugmentationOption option)
		{
			this.controller = controller;
			this.option = option;
			this.councilor = councilor;
		}

		// Token: 0x06004C0E RID: 19470 RVA: 0x001FF690 File Offset: 0x001FD890
		public void UpdateListItem()
		{
			string text;
			string text2;
			string text3;
			string text4;
			this.option.SetAugmentationStrings(out text, out text2, out text3, out text4);
			this.augmentationName.SetText(text2);
			this.augmentationDescription.SetText(text);
			this.augmentationCost.SetText(text4);
			this.augmentationDetails.SetText("BodyText", text3);
			this.selectButton.interactable = this.option.CouncilorCanAfford(this.councilor);
		}

		// Token: 0x06004C0F RID: 19471 RVA: 0x001FF701 File Offset: 0x001FD901
		public void OnAugmentButtonPressed()
		{
			this.controller.OnAugmentationSelected(this.option, this);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
		}

		// Token: 0x06004C10 RID: 19472 RVA: 0x001FF721 File Offset: 0x001FD921
		public void SetSelected(bool selected)
		{
			if (selected)
			{
				this.backgroundImage.sprite = this.selectedBackground;
				return;
			}
			this.backgroundImage.sprite = this.defaultBackground;
		}

		// Token: 0x04002D76 RID: 11638
		private CouncilorAugmentationOption option;

		// Token: 0x04002D77 RID: 11639
		private TICouncilorState councilor;

		// Token: 0x04002D78 RID: 11640
		private CouncilGridController controller;

		// Token: 0x04002D79 RID: 11641
		public Button selectButton;

		// Token: 0x04002D7A RID: 11642
		public TMP_Text augmentationName;

		// Token: 0x04002D7B RID: 11643
		public TMP_Text augmentationDescription;

		// Token: 0x04002D7C RID: 11644
		public TMP_Text augmentationCost;

		// Token: 0x04002D7D RID: 11645
		public TooltipTrigger augmentationDetails;

		// Token: 0x04002D7E RID: 11646
		public Image backgroundImage;

		// Token: 0x04002D7F RID: 11647
		public Sprite defaultBackground;

		// Token: 0x04002D80 RID: 11648
		public Sprite selectedBackground;
	}
}
