using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008A7 RID: 2215
	public class NotificationSummaryItemController : MonoBehaviour
	{
		// Token: 0x06005401 RID: 21505 RVA: 0x0025FCB0 File Offset: 0x0025DEB0
		public void Initialize(NotificationSummaryItem summaryItem)
		{
			this.summaryItem = summaryItem;
		}

		// Token: 0x06005402 RID: 21506 RVA: 0x0025FCBC File Offset: 0x0025DEBC
		public void UpdateListItem()
		{
			this.summaryText.SetText(this.summaryItem.itemSummary);
			string iconResource = this.summaryItem.iconResource;
			string iconBackgroundResource = this.summaryItem.iconBackgroundResource;
			if (!string.IsNullOrEmpty(this.summaryItem.iconResource))
			{
				this.summaryIcon.enabled = true;
				GameControl.assetLoader.LoadAssetForImageAssignment(iconResource, this.summaryIcon);
			}
			else
			{
				this.summaryIcon.enabled = false;
			}
			if (!string.IsNullOrEmpty(this.summaryItem.iconBackgroundResource))
			{
				this.iconBackgroundImage.enabled = true;
				GameControl.assetLoader.LoadAssetForImageAssignment(iconBackgroundResource, this.iconBackgroundImage);
				this.iconBackgroundImage.color = this.summaryItem.backgroundColor;
				return;
			}
			this.iconBackgroundImage.enabled = false;
		}

		// Token: 0x06005403 RID: 21507 RVA: 0x0025FD88 File Offset: 0x0025DF88
		public void OnClicked()
		{
			if (this.summaryItem.gotoGameState != null && this.summaryItem.gotoGameState.exists)
			{
				SoundEffectController.PlaySelectSound(this.summaryItem.gotoGameState);
				TIUtilities.GotoGameState(this.summaryItem.gotoGameState, true, true, true, true, false, -1f);
			}
		}

		// Token: 0x04003A39 RID: 14905
		public NotificationSummaryItem summaryItem;

		// Token: 0x04003A3A RID: 14906
		public TMP_Text summaryText;

		// Token: 0x04003A3B RID: 14907
		public Image summaryIcon;

		// Token: 0x04003A3C RID: 14908
		public Image iconBackgroundImage;

		// Token: 0x04003A3D RID: 14909
		public Color iconBackgroundColor;

		// Token: 0x04003A3E RID: 14910
		public TIGameState gotoGameState;
	}
}
