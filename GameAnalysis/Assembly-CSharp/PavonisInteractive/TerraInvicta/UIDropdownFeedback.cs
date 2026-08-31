using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008F3 RID: 2291
	public class UIDropdownFeedback : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
	{
		// Token: 0x060057EF RID: 22511 RVA: 0x002857EB File Offset: 0x002839EB
		private void Awake()
		{
			this.attachedDropDown = base.GetComponent<TMP_Dropdown>();
		}

		// Token: 0x060057F0 RID: 22512 RVA: 0x002857FC File Offset: 0x002839FC
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.attachedDropDown != null && this.attachedDropDown.enabled && this.attachedDropDown.interactable)
			{
				switch (this.hoverSoundVariant)
				{
				case UIDropdownFeedback.HoverSoundVariant.HoverButton:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverButton", false, false);
					return;
				case UIDropdownFeedback.HoverSoundVariant.HoverButtonList:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverListButton", false, false);
					return;
				case UIDropdownFeedback.HoverSoundVariant.HoverNonButton:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverNonButton", false, false);
					return;
				default:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverButton", false, false);
					break;
				}
			}
		}

		// Token: 0x04003F78 RID: 16248
		private TMP_Dropdown attachedDropDown;

		// Token: 0x04003F79 RID: 16249
		public UIDropdownFeedback.HoverSoundVariant hoverSoundVariant;

		// Token: 0x020011E6 RID: 4582
		public enum HoverSoundVariant
		{
			// Token: 0x04006885 RID: 26757
			HoverButton,
			// Token: 0x04006886 RID: 26758
			HoverButtonList,
			// Token: 0x04006887 RID: 26759
			HoverNonButton
		}
	}
}
