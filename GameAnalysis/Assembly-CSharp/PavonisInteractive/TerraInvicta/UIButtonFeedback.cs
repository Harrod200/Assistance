using System;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008F2 RID: 2290
	public class UIButtonFeedback : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
	{
		// Token: 0x060057EC RID: 22508 RVA: 0x00285752 File Offset: 0x00283952
		private void Awake()
		{
			this.attachedButton = base.GetComponent<Button>();
		}

		// Token: 0x060057ED RID: 22509 RVA: 0x00285760 File Offset: 0x00283960
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.attachedButton != null && this.attachedButton.enabled && this.attachedButton.interactable)
			{
				switch (this.buttonSoundVariant)
				{
				case UIButtonFeedback.ButtonSoundVariant.HoverButton:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverButton", false, false);
					return;
				case UIButtonFeedback.ButtonSoundVariant.HoverButtonList:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverListButton", false, false);
					return;
				case UIButtonFeedback.ButtonSoundVariant.HoverNonButton:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverNonButton", false, false);
					return;
				default:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverButton", false, false);
					break;
				}
			}
		}

		// Token: 0x04003F76 RID: 16246
		private Button attachedButton;

		// Token: 0x04003F77 RID: 16247
		public UIButtonFeedback.ButtonSoundVariant buttonSoundVariant;

		// Token: 0x020011E5 RID: 4581
		public enum ButtonSoundVariant
		{
			// Token: 0x04006881 RID: 26753
			HoverButton,
			// Token: 0x04006882 RID: 26754
			HoverButtonList,
			// Token: 0x04006883 RID: 26755
			HoverNonButton
		}
	}
}
