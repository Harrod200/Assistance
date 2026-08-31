using System;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008F9 RID: 2297
	public class UIToggleFeedback : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
	{
		// Token: 0x060057FA RID: 22522 RVA: 0x00285A3D File Offset: 0x00283C3D
		private void Awake()
		{
			this.attachedToggle = base.GetComponent<Toggle>();
		}

		// Token: 0x060057FB RID: 22523 RVA: 0x00285A4C File Offset: 0x00283C4C
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.attachedToggle != null && this.attachedToggle.enabled && this.attachedToggle.interactable)
			{
				switch (this.hoverSoundVariant)
				{
				case UIToggleFeedback.ButtonSoundVariant.HoverButton:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverButton", false, false);
					return;
				case UIToggleFeedback.ButtonSoundVariant.HoverButtonList:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverListButton", false, false);
					return;
				case UIToggleFeedback.ButtonSoundVariant.HoverNonButton:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverNonButton", false, false);
					return;
				default:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverButton", false, false);
					break;
				}
			}
		}

		// Token: 0x04003F7D RID: 16253
		private Toggle attachedToggle;

		// Token: 0x04003F7E RID: 16254
		public UIToggleFeedback.ButtonSoundVariant hoverSoundVariant;

		// Token: 0x020011E8 RID: 4584
		public enum ButtonSoundVariant
		{
			// Token: 0x0400688E RID: 26766
			HoverButton,
			// Token: 0x0400688F RID: 26767
			HoverButtonList,
			// Token: 0x04006890 RID: 26768
			HoverNonButton
		}
	}
}
