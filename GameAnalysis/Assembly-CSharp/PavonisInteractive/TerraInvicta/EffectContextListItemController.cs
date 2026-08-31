using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008C1 RID: 2241
	internal class EffectContextListItemController : MonoBehaviour
	{
		// Token: 0x0600559E RID: 21918 RVA: 0x0026EADC File Offset: 0x0026CCDC
		public void SetListItem(Context context, ResearchScreenController controller)
		{
			this.effectContext = context;
			this.controller = controller;
			this.selectContextButtonText.SetText(ResearchScreenController.EffectContextToString(this.effectContext));
		}

		// Token: 0x0600559F RID: 21919 RVA: 0x0026EB02 File Offset: 0x0026CD02
		public void OnContextButtonPressed()
		{
			this.controller.OnEffectContextButtonPressed(this.effectContext);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
		}

		// Token: 0x060055A0 RID: 21920 RVA: 0x0026EB21 File Offset: 0x0026CD21
		public void SetSelected(bool selected)
		{
			this.backgroundImage.sprite = (selected ? this.selectedBackground : this.defaultBackground);
		}

		// Token: 0x04003C0A RID: 15370
		private ResearchScreenController controller;

		// Token: 0x04003C0B RID: 15371
		public Button selectContextButton;

		// Token: 0x04003C0C RID: 15372
		public TMP_Text selectContextButtonText;

		// Token: 0x04003C0D RID: 15373
		[HideInInspector]
		public Context effectContext;

		// Token: 0x04003C0E RID: 15374
		public Image backgroundImage;

		// Token: 0x04003C0F RID: 15375
		public Sprite defaultBackground;

		// Token: 0x04003C10 RID: 15376
		public Sprite selectedBackground;
	}
}
