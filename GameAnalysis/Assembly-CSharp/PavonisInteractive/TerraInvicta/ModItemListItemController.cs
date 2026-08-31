using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200088A RID: 2186
	public class ModItemListItemController : MonoBehaviour
	{
		// Token: 0x060051CB RID: 20939 RVA: 0x0023F1BC File Offset: 0x0023D3BC
		public void Init(ModMenuController controller)
		{
			this.controller = controller;
			this.modEnableText.SetText(Loc.T("UI.StartScreen.Mods.Enable"));
			this.modDisableText.SetText(Loc.T("UI.StartScreen.Mods.Disable"));
			this.modDeleteText.SetText(Loc.T("UI.Save.Delete"));
			this.modDisableButton.gameObject.SetActive(true);
			this.modEnableButton.gameObject.SetActive(true);
			this.modDeleteButton.gameObject.SetActive(true);
		}

		// Token: 0x060051CC RID: 20940 RVA: 0x0023F242 File Offset: 0x0023D442
		public void ItemSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
		}

		// Token: 0x060051CD RID: 20941 RVA: 0x0023F250 File Offset: 0x0023D450
		public void OnClickDisable()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			this.controller.modManager.DisableMod(this.modName.text);
		}

		// Token: 0x060051CE RID: 20942 RVA: 0x0023F279 File Offset: 0x0023D479
		public void OnClickEnable()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericSelect", false, false);
			this.controller.modManager.EnableMod(this.modName.text);
		}

		// Token: 0x060051CF RID: 20943 RVA: 0x0023F2A2 File Offset: 0x0023D4A2
		public void OnClickDelete()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Decline", false, false);
			this.controller.modManager.DeleteMod(this.modName.text);
		}

		// Token: 0x060051D0 RID: 20944 RVA: 0x0023F2CC File Offset: 0x0023D4CC
		public void UpdateListItem(ModItemListItemController.ModStatus modStatus, bool steamWorkshop = false)
		{
			this.modStatus = modStatus;
			if (modStatus == ModItemListItemController.ModStatus.Enabled)
			{
				this.modStatusText.SetText(TIUtilities.GreenLine(Loc.T("UI.StartScreen.Mods.Enabled")));
			}
			else
			{
				this.modStatusText.SetText(TIUtilities.RedLine(Loc.T("UI.StartScreen.Mods.Disabled")));
			}
			if (steamWorkshop)
			{
				this.modWorkshopText.SetText(TIUtilities.YellowLine(Loc.T("UI.StartScreen.Mods.Subscribed")));
			}
			else
			{
				this.modWorkshopText.SetText("");
			}
			this.modEnableButton.interactable = modStatus == ModItemListItemController.ModStatus.Disabled;
			this.modDisableButton.interactable = modStatus == ModItemListItemController.ModStatus.Enabled;
			this.modDeleteButton.interactable = modStatus == ModItemListItemController.ModStatus.Disabled;
		}

		// Token: 0x04003665 RID: 13925
		private ModMenuController controller;

		// Token: 0x04003666 RID: 13926
		public TMP_Text modName;

		// Token: 0x04003667 RID: 13927
		public TMP_Text modDesc;

		// Token: 0x04003668 RID: 13928
		public TMP_Text modStatusText;

		// Token: 0x04003669 RID: 13929
		public TMP_Text modEnableText;

		// Token: 0x0400366A RID: 13930
		public TMP_Text modDisableText;

		// Token: 0x0400366B RID: 13931
		public TMP_Text modDeleteText;

		// Token: 0x0400366C RID: 13932
		public TMP_Text modWorkshopText;

		// Token: 0x0400366D RID: 13933
		public Button modEnableButton;

		// Token: 0x0400366E RID: 13934
		public Button modDisableButton;

		// Token: 0x0400366F RID: 13935
		public Button modDeleteButton;

		// Token: 0x04003670 RID: 13936
		public Image enabledImage;

		// Token: 0x04003671 RID: 13937
		public ModItemListItemController.ModStatus modStatus;

		// Token: 0x020010DD RID: 4317
		public enum ModStatus
		{
			// Token: 0x0400657A RID: 25978
			Enabled,
			// Token: 0x0400657B RID: 25979
			Disabled
		}
	}
}
